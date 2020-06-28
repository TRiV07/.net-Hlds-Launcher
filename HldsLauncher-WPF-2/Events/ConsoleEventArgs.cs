using System;

namespace HldsLauncher.Events
{
    public class ConsoleEventArgs : EventArgs
    {
        public string Console { get; set; }
        public string Message { get; set; }
    }
}
