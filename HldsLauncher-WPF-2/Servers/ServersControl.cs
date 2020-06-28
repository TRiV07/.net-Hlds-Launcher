using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using HldsLauncher.Options;
using System.Collections.ObjectModel;
using HldsLauncher.Events;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using HldsLauncher.Log;
using HldsLauncher.Utils;

namespace HldsLauncher.Servers
{
    public class ServersControl : IServersControl
    {
        private int _newServerId = 0;
        private Thread _opertaionThread;
        protected ILogger _logger;
        public ObservableCollection<IServer> Servers { get; set; }
        public event EventHandler<ServersControlEventArgs> ProgressUpdated;

        private NamedPipeServerStream _pipeServer = null;
        private Thread _dialogBoxErrorReceiveThread = null;

        public ServersControl()
        {
            _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();
            Servers = new ObservableCollection<IServer>();

            _pipeServer = new NamedPipeServerStream("andOneHldsLauncher2");
            _dialogBoxErrorReceiveThread = ThreadUtil.GetThread(DialogBoxErrorReceive);
            _dialogBoxErrorReceiveThread.IsBackground = true;
            _dialogBoxErrorReceiveThread.Start();
        }

        public void AddFromXml(XNode xServers)
        {
            foreach (var xServer in xServers.XPathSelectElements("Server"))
            {
                switch (xServer.XPathSelectElement("Type").Value)
                {
                    case "GoldSource":
                        {
                            Servers.Add(BaseServer.CreateFromXml<GoldSourceServer, GoldSourceServerOptions>(xServer));
                            _newServerId++;
                            break;
                        }
                    case "Source":
                        {
                            Servers.Add(BaseServer.CreateFromXml<SourceServer, SourceServerOptions>(xServer));
                            _newServerId++;
                            break;
                        }
                    case "Hltv":
                        {
                            Servers.Add(BaseServer.CreateFromXml<ValveHltvServer, ValveHltvServerOptions>(xServer));
                            _newServerId++;
                            break;
                        }
                }
            }
        }

        public XNode GetXml()
        {
            XElement xServers = new XElement("Servers");
            foreach (var server in Servers)
            {
                xServers.Add(server.GetXml());
            }
            return xServers;
        }

        public void StartAll(int interval)
        {
            if (_opertaionThread.ThreadState == ThreadState.Running || _opertaionThread.ThreadState == ThreadState.WaitSleepJoin)
            {
                return;
            }
            _opertaionThread = ThreadUtil.GetThread(() =>
            {
                int serversDone = 0;
                foreach (IServer server in Servers)
                {
                    serversDone++;
                    if (ProgressUpdated != null)
                    {
                        ProgressUpdated(this, new ServersControlEventArgs() { ServersDone = serversDone, ServersCount = Servers.Count });
                    }
                    if (!server.ServerStatus.Status)
                    {
                        server.Start();
                        Thread.Sleep(interval);
                    }
                }
            });
            _opertaionThread.Start();
        }

        public void StartAllActive(int interval)
        {
            if (_opertaionThread != null)
            {
                if (_opertaionThread.ThreadState == ThreadState.Running || _opertaionThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return;
                }
            }
            _opertaionThread = ThreadUtil.GetThread(() =>
            {
                int serversDone = 0;
                int activeServerCount = 0;
                foreach (IServer server in Servers)
                {
                    if (server.Options.ActiveServer)
                    {
                        activeServerCount++;
                    }
                }
                foreach (var server in Servers)
                {
                    if (server.Options.ActiveServer)
                    {
                        serversDone++;
                        if (ProgressUpdated != null)
                        {
                            ProgressUpdated(this, new ServersControlEventArgs() { ServersDone = serversDone, ServersCount = activeServerCount });
                        }
                        if (!server.ServerStatus.Status)
                        {
                            server.Start();
                            Thread.Sleep(interval);
                        }
                    }
                }
            });
            _opertaionThread.Start();
        }

        public void DelayedStartAllActive(int interval, int delay)
        {
            if (_opertaionThread != null)
            {
                if (_opertaionThread.ThreadState == ThreadState.Running || _opertaionThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return;
                }
            }
            _opertaionThread = ThreadUtil.GetThread(() =>
            {
                Thread.Sleep(delay);
                int serversDone = 0;
                foreach (var server in Servers)
                {
                    serversDone++;
                    if (ProgressUpdated != null)
                    {
                        ProgressUpdated(this, new ServersControlEventArgs() { ServersDone = serversDone, ServersCount = Servers.Count });
                    }
                    if (server.Options.ActiveServer)
                    {
                        if (!server.ServerStatus.Status)
                        {
                            server.Start();
                            Thread.Sleep(interval);
                        }
                    }
                }
            });
            _opertaionThread.Start();
        }

        public void StopAll()
        {
            int serversDone = 0;
            foreach (var server in Servers)
            {
                server.Stop();
                serversDone++;
                if (ProgressUpdated != null)
                {
                    ProgressUpdated(this, new ServersControlEventArgs() { ServersDone = serversDone, ServersCount = Servers.Count });
                }
            }
        }

        public void KillAll()
        {
            foreach (var server in Servers)
            {
                try
                {
                    server.Kill();
                }
                catch { }
            }
        }

        public void StopAllInOtherThread()
        {
            if (_opertaionThread != null)
            {
                if (_opertaionThread.ThreadState == ThreadState.Running || _opertaionThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    return;
                }
            }
            _opertaionThread = ThreadUtil.GetThread(StopAll);
            _opertaionThread.Start();
        }

        public void CancelOperation()
        {
            if (_opertaionThread != null)
            {
                _opertaionThread.Abort();
            }
        }

        private void DialogBoxErrorReceive()
        {
            BinaryReader br;
            Encoding enc = new UnicodeEncoding(false, true, false);
            while (true)
            {
                try
                {
                    _pipeServer.WaitForConnection();
                    br = new BinaryReader(_pipeServer);
                    String receivedStr = "";
                    byte[] receivedBytes = new byte[2048];
                    int len = br.Read(receivedBytes, 0, 1024);
                    int pos = len;
                    while (len > 0)
                    {
                        len = br.Read(receivedBytes, pos, 1024);
                        pos += len;
                    }
                    receivedStr += enc.GetString(receivedBytes, 0, pos);

                    br.Close();
                    _pipeServer.Close();

                    int processId = int.Parse(receivedStr.Split('|')[0]);
                    String caption = receivedStr.Split('|')[1];
                    String text = receivedStr.Split('|')[2];

                    IServer server = (from serv in Servers
                                      where serv.ServerProcess.Id == processId
                                      select serv).FirstOrDefault();

                    if (server != null)
                    {
                        server.CrashWithDialog(caption, text);
                    }
                }
                catch (Exception e)
                {
                    _logger.DebugException("DialogBoxErrorReceive error", e);
                }
                finally
                {
                    _pipeServer = new NamedPipeServerStream("andOneHldsLauncher2");
                }
            }
        }
    }
}
