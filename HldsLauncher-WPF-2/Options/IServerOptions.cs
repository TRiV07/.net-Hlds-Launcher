using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public interface IServerOptions
    {
        string ExecutablePath { get; set; }
        ProcessPriorityClass Priority { get; set; }
        IntPtr ProcessorAffinity { get; set; }
        bool AutoRestart { get; set; }
        bool ActiveServer { get; set; }
        bool CrashCountLimit { get; set; }
        int MaxCrashCount { get; set; }
        int CrashCountTime { get; set; }
        ServerType Type { get; }
        string AdditionalCommandLineArgs { get; set; }
        string HostName { get; set; }

        string CommandLine();
        XNode GetXml();
    }
}
