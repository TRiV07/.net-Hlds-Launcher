using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using HldsLauncher.Utils;
using System.Runtime.InteropServices;
using HldsLauncher.Options;
using HldsLauncher.Log;

namespace HldsLauncher.Servers
{
    public class ValveHltvServer : BaseServer
    {
        ValveHltvServerStatus _serverStatus;
        public override IServerStatus ServerStatus { get { return _serverStatus; } set { _serverStatus = value as ValveHltvServerStatus; } }
        private ILogger _logger;


        public ValveHltvServer()
        {
            _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();
            ServerStatus = new ValveHltvServerStatus();
            Options = new ValveHltvServerOptions();
        }

        protected void resetStatus()
        {
            ServerStatus.Reset();
        }

        public override void Start()
        {
            resetStatus();
            base.Start();
        }

        public override void Stop()
        {
            resetStatus();
            base.Stop();
        }

        protected override void Run()
        {
            ServerProcess = new Process();
            FileInfo executablePath = new FileInfo(Options.ExecutablePath);
            if (executablePath.Exists)
            {
                ServerProcess.StartInfo.FileName = executablePath.FullName;
                ServerProcess.StartInfo.WorkingDirectory = executablePath.Directory.FullName;
                ServerProcess.StartInfo.Arguments = Options.CommandLine();
                ServerProcess.StartInfo.UseShellExecute = false;

                do
                {
                    ServerProcess.Start();
                    ServerProcess.WaitForInputIdle(2000);

                    WinAPI.SetWindowPos(ServerProcess.MainWindowHandle,
                            (IntPtr)WinAPI.HWND_TOP,
                            (Options as ValveHltvServerOptions).ConsolePositionX,
                            (Options as ValveHltvServerOptions).ConsolePositionY,
                            0, 0, WinAPI.SWP_NOSIZE);

                    if (Options.ProcessorAffinity != (IntPtr)0 && ServerProcess.Responding)
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
                    if (ServerProcess.Responding)
                    {
                        try
                        {
                            ServerProcess.PriorityClass = Options.Priority;
                        }
                        catch (Exception e)
                        {
                            _logger.DebugException(Properties.Resources.log_UnableToSetPriority, e);
                        }
                    }

                    ServerStatus.Status = true;
                    _logger.Info(string.Format(Properties.Resources.log_SuccessfulStarted, Options.HostName));
                    ServerProcess.WaitForExit();
                    ServerStatus.Status = false;
                    CrashWithoutDialog();
                }
                while (Options.AutoRestart);
            }
            else
            {
                _logger.Warn(string.Format(Properties.Resources.log_StartFailedFileIsNotExists, Options.ExecutablePath));
            }
        }
    }
}
