using System;
using HldsLauncher.Servers;

namespace HldsLauncher.Events
{
    public class ServerInfoEventArgs : EventArgs
    {
        public IServerStatus ServerStatus { get; set; }
    }
}
