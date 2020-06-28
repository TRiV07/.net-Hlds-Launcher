using System;
using System.Xml.Linq;
using HldsLauncher.Events;
using HldsLauncher.Options;
using HldsLauncher.Statistics;
using System.Diagnostics;

namespace HldsLauncher.Servers
{
    public interface IServer
    {
        IServerOptions Options { get; set; }
        IServerStatistics Statistics { get; set; }
        Process ServerProcess { get; set; }
        string ConsoleText { get; set; }
        IServerStatus ServerStatus { get; set; }
        event EventHandler<ConsoleEventArgs> ConsoleUpdated;
        event EventHandler<ServerInfoEventArgs> ServerInfoUpdated;
        event EventHandler<EventArgs> ServerStoped;
        event EventHandler<EventArgs> ServerStarted;
        void Start();
        void Stop();
        void Kill();
        void CrashWithDialog(string caption, string text);
        XNode GetXml();
    }
}