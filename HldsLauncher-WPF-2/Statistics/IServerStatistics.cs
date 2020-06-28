using System;
using System.Collections.Generic;

namespace HldsLauncher.Statistics
{
    public interface IServerStatistics
    {
        List<CrashInfo> Crashes { get; set; }

        void AddCrashInfo(string map);
        void Reset();
    }
}
