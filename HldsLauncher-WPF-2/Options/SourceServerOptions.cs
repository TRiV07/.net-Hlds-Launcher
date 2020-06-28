using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public class SourceServerOptions : ValveGameServerOptions
    {
        private SourceGame _game;
        public SourceGame Game { get { return _game; } set { _game = value; OnPropertyChanged("Game"); } }
        private int _tickRate;
        public int TickRate { get { return _tickRate; } set { _tickRate = value; OnPropertyChanged("TickRate"); } }

        public SourceServerOptions()
        {
            Type = ServerType.Source;

            ArgsSufix = "-";
            Game = SourceGame.cstrike;
            TickRate = 66;
        }

        public override string CommandLine()
        {
            return (" -game " + Game.ToString()) +
                    (" -tickrate " + TickRate.ToString()) +
                    base.CommandLine();
        }
    }
}
