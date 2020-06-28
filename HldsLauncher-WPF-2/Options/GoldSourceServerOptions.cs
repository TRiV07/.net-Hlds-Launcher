using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public class GoldSourceServerOptions : ValveGameServerOptions
    {
        private GoldSourceGame _game;
        public GoldSourceGame Game { get { return _game; } set { _game = value; OnPropertyChanged("Game"); } }

        public GoldSourceServerOptions()
        {
            Type = ServerType.GoldSource;

            ArgsSufix = "+";
            Game = GoldSourceGame.cstrike;
        }

        public override string CommandLine()
        {
            return (" -game " + Game.ToString()) +
                base.CommandLine();
        }
    }
}
