using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public class ValveHltvServerOptions : ValveServerOptions
    {
        public ValveHltvServerOptions()
        {
            Type = ServerType.Hltv;
            ArgsSufix = "-";
        }
        
        public override string CommandLine()
        {
            return base.CommandLine();
        }
    }
}
