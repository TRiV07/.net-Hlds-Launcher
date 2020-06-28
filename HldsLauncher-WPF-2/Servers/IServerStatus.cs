using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Servers
{
    public interface IServerStatus
    {
        bool Status { get; set; }
        double Fps { get; set; }
        string Map { get; set; }
        int MaxPlayers { get; set; }
        int OnlinePlayers { get; set; }
        string OnlineToMaxPlayers { get; }
        void Reset();
    }
}
