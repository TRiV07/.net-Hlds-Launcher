using System;

namespace HldsLauncher.Events
{
    public class ServersControlEventArgs : EventArgs
    {
        public int ServersDone { get; set; }
        public int ServersCount { get; set; }
    }
}
