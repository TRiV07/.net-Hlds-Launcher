using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Events;
using System.Threading;
using HldsLauncher.Utils;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using HldsLauncher.Options;
using System.Text.RegularExpressions;
using HldsLauncher.Enums;
using HldsLauncher.Log;
using System.Globalization;
using HldsLauncher.Statistics;

namespace HldsLauncher.Servers
{
    public abstract class ValveGameServer : BaseServer
    {
        protected int _hHook = 0;
        protected IntPtr[] _hHookBufInfo = new IntPtr[3];
        protected Thread _refreshInfoThread;
        private ILogger _logger;

        private Encoding _encoding = Encoding.GetEncoding(65001, new EncoderReplacementFallback(" "), new DecoderReplacementFallback(" "));
        //private Encoding _encoding = Encoding.UTF8;
        private bool _stoping = false;

        ValveGameServerStatus _serverStatus;
        public override IServerStatus ServerStatus { get { return _serverStatus; } set { _serverStatus = value as ValveGameServerStatus; } }

        protected ValveGameServer()
        {
            _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();
            ServerStatus = new ValveGameServerStatus();
            //_serverThread = new Thread(run);
        }

        protected void ResetStatus()
        {
            ServerStatus.Reset();
        }

        public override void Start()
        {
            _stoping = false;
            _crashWithDialog = false;
            if (_serverThread == null)
            {
                ResetStatus();
                _refreshInfoThread = ThreadUtil.GetThread(RefreshText);
            }
            else if (_serverThread.ThreadState != System.Threading.ThreadState.Running && _serverThread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
            {
                ResetStatus();
                _refreshInfoThread = ThreadUtil.GetThread(RefreshText);
            }
            base.Start();
        }

        protected override void Run()
        {
            ServerProcess = new Process();
            if (!string.IsNullOrEmpty(Options.ExecutablePath))
            {
                FileInfo executablePath = new FileInfo(Options.ExecutablePath);
                if (executablePath.Exists)
                {
                    string injectError = "";
                    WinAPI.SECURITY_ATTRIBUTES SA = new WinAPI.SECURITY_ATTRIBUTES();
                    SA.nLength = (uint)(Marshal.SizeOf(typeof(WinAPI.SECURITY_ATTRIBUTES)));
                    SA.bInheritHandle = true;
                    SA.lpSecurityDescriptor = IntPtr.Zero;
                    IntPtr SAPtr = Marshal.AllocHGlobal(Marshal.SizeOf(SA));
                    Marshal.StructureToPtr(SA, SAPtr, true);

                    _hHookBufInfo[0] = WinAPI.CreateFileMapping((IntPtr)(-1), SAPtr, WinAPI.FileMapProtection.PageReadWrite, 0, 16384, null);
                    _hHookBufInfo[1] = WinAPI.CreateEvent(SAPtr, false, false, null);
                    _hHookBufInfo[2] = WinAPI.CreateEvent(SAPtr, false, false, null);

                    ServerProcess.StartInfo.FileName = executablePath.FullName;
                    ServerProcess.StartInfo.WorkingDirectory = executablePath.Directory.FullName;
                    ServerProcess.StartInfo.Arguments = string.Format("{0} -HFILE {1} -HPARENT {2} -HCHILD {3} -conheight 24",
                        Options.CommandLine(),
                        _hHookBufInfo[0],
                        _hHookBufInfo[1],
                        _hHookBufInfo[2]);
                    ServerProcess.StartInfo.UseShellExecute = false;

                    do
                    {
                        ServerProcess.Start();
                        Thread.Sleep(50);
                        try
                        {
                            DllHookUtil.CreateHook(CommonUtils.GetProgramDirectory() + @"\Dlls\ErrorsHook.dll", ref _hHook, ServerProcess.Threads[0].Id);
                        }
                        catch (Exception e)
                        {
                            _logger.DebugException(Properties.Resources.log_UnableToCreateHook, e);
                        }
                        try
                        {
                            ServerProcess.WaitForInputIdle(5000);
                        }
                        catch (Exception e)
                        {
                            _logger.DebugException(Properties.Resources.log_UnableToSetAffinity, e);
                        }
                        DllInjectUtil.DoInject(ServerProcess, CommonUtils.GetProgramDirectory() + @"\Dlls\Inject.dll", out injectError);

                        if ((Options as ValveGameServerOptions).ConsoleType == ConsoleType.Integrated)
                        {
                            WinAPI.ShowWindow(ServerProcess.MainWindowHandle, WinAPI.SW_HIDE);
                        }
                        else
                        {
                            WinAPI.SetWindowPos(ServerProcess.MainWindowHandle,
                                (IntPtr)WinAPI.HWND_TOP,
                                (Options as ValveGameServerOptions).ConsolePositionX,
                                (Options as ValveGameServerOptions).ConsolePositionY,
                                0, 0, WinAPI.SWP_NOSIZE);
                        }

                        if (Options.ProcessorAffinity != (IntPtr)0)
                        {
                            try
                            {
                                ServerProcess.ProcessorAffinity = Options.ProcessorAffinity;
                            }
                            catch (Exception e)
                            {
                                _logger.DebugException(Properties.Resources.log_UnableToSetAffinity, e);
                            }
                        }
                        try
                        {
                            ServerProcess.PriorityClass = Options.Priority;
                        }
                        catch (Exception e)
                        {
                            _logger.DebugException(Properties.Resources.log_UnableToSetPriority, e);
                        }
                        _refreshInfoThread.Start();
                        _logger.Info(string.Format(Properties.Resources.log_SuccessfulStarted, Options.HostName));
                        ServerStatus.Status = true;
                        OnServerStarted(this, new EventArgs());
                        ServerProcess.WaitForExit();

                        ServerStatus.Status = false;
                        _refreshInfoThread.Abort();
                        _refreshInfoThread = ThreadUtil.GetThread(RefreshText);

                        CrashWithoutDialog();

                        if (!string.IsNullOrWhiteSpace(ServerStatus.Map))
                        {
                            ServerProcess.StartInfo.Arguments = string.Format("{0} -HFILE {1} -HPARENT {2} -HCHILD {3} -conheight 24",
                                Options.CommandLine().Replace((Options as ValveGameServerOptions).Map, ServerStatus.Map),
                                _hHookBufInfo[0],
                                _hHookBufInfo[1],
                                _hHookBufInfo[2]);
                        }

                        ResetStatus();
                        Statistics.AddCrashInfo(ServerStatus.Map);
                        DllHookUtil.CloseHook(_hHook);
                        if (!CanRestart())
                        {
                            _logger.Info(string.Format(Properties.Resources.log_CrashCountMaxLimit, Options.HostName, Options.MaxCrashCount, Options.CrashCountTime));
                            
                            break;
                        }
                    }
                    while (Options.AutoRestart);
                    OnServerStoped(this, new EventArgs());
                }
                else
                {
                    _logger.Warn(string.Format(Properties.Resources.log_StartFailedFileIsNotExists, Options.ExecutablePath));
                }
            }
            else
            {
                _logger.Warn(string.Format(Properties.Resources.log_StartFailedFileIsNotExists, Options.ExecutablePath));
            }
        }

        public override void Stop()
        {
            _stoping = true;
            if (_refreshInfoThread != null)
            {
                if (_refreshInfoThread.ThreadState != System.Threading.ThreadState.Unstarted)
                {
                    _refreshInfoThread.Join(500);
                    _refreshInfoThread.Abort();
                }
            }
            closeHooks();
            ResetStatus();
            base.Stop();
        }

        private bool CanRestart()
        {
            if (!Options.CrashCountLimit) return true;

            if (Statistics.Crashes.Count < Options.MaxCrashCount) return true;

            CrashInfo[] crashes = Statistics.Crashes
                .OrderByDescending(x => x.Time)
                .Take(Options.MaxCrashCount)
                .OrderBy(x => x.Time).ToArray();

            CrashInfo testCrash = crashes.FirstOrDefault();
            if (testCrash == null) return true;

            return testCrash.Time.AddSeconds(Options.CrashCountTime) < DateTime.Now;
        }

        private void closeHooks()
        {
            if (_hHookBufInfo[0] != (IntPtr)0)
            {
                WinAPI.UnmapViewOfFile(_hHookBufInfo[0]);
                WinAPI.CloseHandle(_hHookBufInfo[0]);
                _hHookBufInfo[0] = (IntPtr)0;
            }
            if (_hHookBufInfo[1] != (IntPtr)0)
            {
                WinAPI.CloseHandle(_hHookBufInfo[1]);
                _hHookBufInfo[1] = (IntPtr)0;
            }
            if (_hHookBufInfo[2] != (IntPtr)0)
            {
                WinAPI.CloseHandle(_hHookBufInfo[2]);
                _hHookBufInfo[2] = (IntPtr)0;
            }
            if (_hHook != 0)
            {
                DllHookUtil.CloseHook(_hHook);
                _hHook = 0;
            }
        }

        private unsafe void RefreshText()
        {
            int width = 80;
            int height = 199;
            StringBuilder stringBuilder = new StringBuilder(width * height);
            byte[] tempRes = new byte[width * height];
            ConsoleEventArgs consoleEventArgs = new ConsoleEventArgs();
            ServerInfoEventArgs serverInfoEventArgs = new ServerInfoEventArgs();
            char* sz;
            while (!_stoping)
            {
                int i = 0;
                sz = (char*)WinAPI.MapViewOfFile(_hHookBufInfo[0], WinAPI.FileMapAccess.FileMapRead | WinAPI.FileMapAccess.FileMapWrite, 0, 0, 0);

                // Command token
                *(int*)&sz[i] = WinAPI.ENGINE_RETRIEVE_CONSOLE_CONTENTS;
                i += 2;

                // Start at line 0
                *(int*)&sz[i] = 0;
                i += 2;

                // End at line 23 ( assumes 24 line console )
                *(int*)&sz[i] = height;
                i += 2;

                bool tt = WinAPI.UnmapViewOfFile(sz);

                if (!_stoping)
                {
                    WinAPI.SetEvent(_hHookBufInfo[1]);

                    Thread.Sleep(20);

                    if (WinAPI.WaitForSingleObject(_hHookBufInfo[2], 0) == WinAPI.WAIT_OBJECT_0)
                    {
                        IntPtr pBuffer = WinAPI.MapViewOfFile(_hHookBufInfo[0], WinAPI.FileMapAccess.FileMapRead | WinAPI.FileMapAccess.FileMapWrite, 0, 0, 0);

                        Marshal.Copy((IntPtr)((int)pBuffer + 4), tempRes, 0, width * height);

                        for (int z = 0; z < tempRes.Length && !_stoping; z += width)
                        {
                            string line = string.Format("{0}\n", _encoding.GetString(tempRes, z, width));
                            if (line.Length < width)
                            {
                                if (z + width < tempRes.Length && tempRes[z + width - 1] != 32)
                                {
                                    line = string.Format("{0}\n", _encoding.GetString(tempRes, z, width + 1));
                                }
                            }
                            stringBuilder.Append(line);
                        }
                        String result = stringBuilder.ToString().Replace("\0", " ");
                        stringBuilder.Clear();

                        if (!_stoping)
                        {
                            string firstLine = result.Length > 80 ? result.Substring(0, 80) : result;
                            try
                            {
                                ParseFirstLine(firstLine);
                            }
                            catch (Exception e)
                            {
                                _logger.DebugException("RefreshText error", e);
                            }

                            consoleEventArgs.Console = result;
                            OnConsoleUpdated(this, consoleEventArgs);
                            serverInfoEventArgs.ServerStatus = ServerStatus;
                            OnServerInfoUpdated(this, serverInfoEventArgs);

                            WinAPI.UnmapViewOfFile(pBuffer);
                            Thread.Sleep(480);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        protected abstract void ParseFirstLine(string firstLine);
    }
}
