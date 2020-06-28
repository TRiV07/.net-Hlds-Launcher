using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public abstract class ValveGameServerOptions : ValveServerOptions
    {
        private ConsoleType _consoleType;
        public ConsoleType ConsoleType { get { return _consoleType; } set { _consoleType = value; OnPropertyChanged("ConsoleType"); } }

        private string _map;
        public string Map { get { return _map; } set { _map = value; OnPropertyChanged("Map"); } }

        private int _maxPlayers;
        public int MaxPlayers { get { return _maxPlayers; } set { _maxPlayers = value; OnPropertyChanged("MaxPlayers"); } }

        private string _rconPassword;
        public string RconPassword { get { return _rconPassword; } set { _rconPassword = value; OnPropertyChanged("RconPassword"); } }

        private bool _vac;
        public bool Vac { get { return _vac; } set { _vac = value; OnPropertyChanged("Vac"); } }

        private bool _noIpx;
        public bool NoIpx { get { return _noIpx; } set { _noIpx = value; OnPropertyChanged("NoIpx"); } }

        private bool _noMaster;
        public bool NoMaster { get { return _noMaster; } set { _noMaster = value; OnPropertyChanged("NoMaster"); } }

        private bool _svLan;
        public bool SvLan { get { return _svLan; } set { _svLan = value; OnPropertyChanged("SvLan"); } }

        protected ValveGameServerOptions()
        {
            ConsoleType = ConsoleType.Integrated;

            Map = "de_dust2";
            MaxPlayers = 12;
            RconPassword = "turboPass";
            Vac = true;
            NoIpx = false;
            NoMaster = false;
            SvLan = false;
        }

        public override string CommandLine()
        {
            return " -console" +
                    ((!string.IsNullOrWhiteSpace(Map)) ? " +map \"" + Map + "\"" : "") +
                    (" " + ArgsSufix + "maxplayers " + MaxPlayers.ToString()) +
                    ((!string.IsNullOrWhiteSpace(RconPassword)) ? " +rcon_password \"" + RconPassword + "\"" : "") +
                    ((Vac) ? "" : " -insecure") +
                    ((NoIpx) ? " -noipx" : "") +
                    ((NoMaster) ? " -nomaster" : "") +
                    ((SvLan) ? " +sv_lan" : "") +
                base.CommandLine();
        }
    }
}