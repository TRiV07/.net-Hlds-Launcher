using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Events;
using HldsLauncher.Options;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using HldsLauncher.Statistics;
using HldsLauncher.Utils;
using HldsLauncher.Log;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;

namespace HldsLauncher.Servers
{
    public abstract class BaseServer : IServer
    {
        protected Thread _serverThread = null;
        protected bool _crashWithDialog = false;
        private ILogger _logger;

        public IServerOptions Options { get; set; }
        public IServerStatistics Statistics { get; set; }
        public Process ServerProcess { get; set; }
        public string ConsoleText { get; set; }

        public virtual IServerStatus ServerStatus { get; set; }

        public event EventHandler<ConsoleEventArgs> ConsoleUpdated;
        protected void OnConsoleUpdated(object sender, ConsoleEventArgs e)
        {
            ConsoleText = e.Console;
            if (ConsoleUpdated != null)
            {
                ConsoleUpdated(sender, e);
            }
        }

        public event EventHandler<ServerInfoEventArgs> ServerInfoUpdated;
        protected void OnServerInfoUpdated(object sender, ServerInfoEventArgs e)
        {
            if (ServerInfoUpdated != null)
            {
                ServerInfoUpdated(sender, e);
            }
        }

        public event EventHandler<EventArgs> ServerStoped;
        protected void OnServerStoped(object sender, EventArgs e)
        {
            if (ServerStoped != null)
            {
                ServerStoped(sender, e);
            }
        }

        public event EventHandler<EventArgs> ServerStarted;
        protected void OnServerStarted(object sender, EventArgs e)
        {
            if (ServerStarted != null)
            {
                ServerStarted(sender, e);
            }
        }

        public BaseServer()
        {
            ConsoleText = "";
            _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();
            Statistics = new ServerStatistics();
        }

        protected abstract void Run();
        public virtual void Start()
        {
            if (_serverThread == null)
            {
                _logger.Info(string.Format(Properties.Resources.log_TryingToStart, Options.HostName));
                Statistics.Reset();
                _serverThread = ThreadUtil.GetThread(Run);
                _serverThread.Start();
            }
            else if (_serverThread.ThreadState != System.Threading.ThreadState.Running && _serverThread.ThreadState != System.Threading.ThreadState.WaitSleepJoin)
            {
                _logger.Info(string.Format(Properties.Resources.log_TryingToStart, Options.HostName));
                Statistics.Reset();
                _serverThread = ThreadUtil.GetThread(Run);
                _serverThread.Start();
            }
            else
            {
                _logger.Info(string.Format(Properties.Resources.log_ServerAlreadyStarted, Options.HostName));
            }
        }

        public virtual void Stop()
        {
            if (_serverThread != null)
            {
                if (_serverThread.ThreadState == System.Threading.ThreadState.Running || _serverThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                {
                    _serverThread.Abort();

                    if (ServerProcess != null)
                    {
                        try
                        {
                            if (this is ValveHltvServer)
                            {
                                _logger.Info(string.Format(Properties.Resources.log_KillProcessShutdown, Options.HostName));
                                try
                                {
                                    ServerProcess.Kill();
                                }
                                catch (Exception e)
                                {
                                    _logger.DebugException(Properties.Resources.log_ProcessAlreadyKilled, e);
                                }
                            }
                            else
                            {
                                _logger.Info(string.Format(Properties.Resources.log_TryingNormalShutdown, Options.HostName));
                                if (SendMessageUtil.SendTextMessage("quit", ServerProcess))
                                {
                                    ServerProcess.WaitForExit(3000);
                                    if (!ServerProcess.HasExited)
                                    {
                                        _logger.Warn(string.Format(Properties.Resources.log_NormalShutdownFailed, Options.HostName));
                                        _logger.Info(string.Format(Properties.Resources.log_KillProcessShutdown, Options.HostName));
                                        try
                                        {
                                            ServerProcess.Kill();
                                        }
                                        catch (Exception e)
                                        {
                                            _logger.DebugException(Properties.Resources.log_ProcessAlreadyKilled, e);
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        ServerProcess.Kill();
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.DebugException(Properties.Resources.log_ProcessAlreadyKilled, e);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                ServerProcess.Kill();
                            }
                            catch (Exception e)
                            {
                                _logger.DebugException(Properties.Resources.log_ProcessAlreadyKilled, e);
                            }
                        }
                    }
                    _logger.Info(string.Format(Properties.Resources.log_SuccessfulStopped, Options.HostName));
                    OnServerStoped(this, new EventArgs());
                }
                else
                {
                    _logger.Info(string.Format(Properties.Resources.log_ServerAlreadyStopped, Options.HostName));
                }
            }
            else
            {
                _logger.Info(string.Format(Properties.Resources.log_ServerAlreadyStopped, Options.HostName));
            }
        }

        public virtual void Kill()
        {
            if (_serverThread != null)
            {
                _serverThread.Abort();
            }
            try
            {
                ServerProcess.Kill();
            }
            catch { }
        }

        public static TServer CreateFromXml<TServer, TOptions>(XNode xServer)
            where TServer : IServer//, new()
            where TOptions : IServerOptions
        {
            TServer server = Activator.CreateInstance<TServer>();
            //T server = new T();
            server.Options = BaseServerOptions.CreateFromXml<TOptions>(xServer);
            return server;
        }

        public XNode GetXml()
        {
            return Options.GetXml();
        }

        protected void CrashWithoutDialog()
        {
            if (!_crashWithDialog)
            {
                if (!(this is ValveHltvServer) && ServerStatus.Map.Length > 0)
                {
                    _logger.Warn(string.Format(Properties.Resources.log_ServerCrashedOnMap.Replace("\\n", "\n"), Options.HostName, ServerStatus.Map, ConsoleText.Trim()));
                }
                else
                {
                    _logger.Warn(string.Format(Properties.Resources.log_ServerCrashed.Replace("\\n", "\n"), Options.HostName, ConsoleText.Trim()));
                }
            }
            _crashWithDialog = false;
        }

        public void CrashWithDialog(string caption, string text)
        {
            _crashWithDialog = true;
            int k = 0;
            while (!ServerStatus.Status && k < 40)
            {
                Thread.Sleep(50);
                k++;
            }
            if (!(this is ValveHltvServer) && ServerStatus.Map.Length > 0)
            {
                _logger.Warn(string.Format(Properties.Resources.log_ServerCrashedWithDialogOnMap.Replace("\\n", "\n"), Options.HostName, ServerStatus.Map, ConsoleText.Trim(), caption, text));
            }
            else
            {
                _logger.Warn(string.Format(Properties.Resources.log_ServerCrashedWithDialog.Replace("\\n", "\n"), Options.HostName, ConsoleText.Trim(), caption, text));
            }
            //while (_serverThread.ThreadState != System.Threading.ThreadState.WaitSleepJoin && k < 40)
            //{
            //    Thread.Sleep(50);
            //    k++;
            //}
            try
            {
                ServerProcess.Kill();
            }
            catch(Exception e)
            {
                _logger.DebugException(Properties.Resources.log_ProcessAlreadyKilled, e);
            }
            Thread.Sleep(100);
        }
    }
}
