using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Enums;

namespace HldsLauncher.Events
{
    public class HldsUpdateEventArgs : EventArgs
    {
        public HldsUpdateStatus UpdateStatus { get; set; }
        public string NewVersion { get; set; }
        public string UpdateUrl { get; set; }
    }
}
