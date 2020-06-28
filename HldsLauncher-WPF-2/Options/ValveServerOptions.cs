using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Options
{
    public abstract class ValveServerOptions : BaseServerOptions
    {
        private int _consolePositionX;
        public int ConsolePositionX { get { return _consolePositionX; } set { _consolePositionX = value; OnPropertyChanged("ConsolePositionX"); } }
        private int _consolePositionY;
        public int ConsolePositionY { get { return _consolePositionY; } set { _consolePositionY = value; OnPropertyChanged("ConsolePositionY"); } }

        private string _argsSufix;
        protected string ArgsSufix { get { return _argsSufix; } set { _argsSufix = value; OnPropertyChanged("ArgsSufix"); } }

        private string _ip;
        public string Ip { get { return _ip; } set { _ip = value; OnPropertyChanged("Ip"); } }
        private string _port;
        public string Port { get { return _port; } set { _port = value; OnPropertyChanged("Port"); } }

        protected ValveServerOptions()
        {
            ConsolePositionX = 30;
            ConsolePositionY = 30;

            ArgsSufix = "+";

            Ip = "0.0.0.0";
            Port = "27015";
        }

        public override string CommandLine()
        {
            return ((!string.IsNullOrWhiteSpace(Ip)) ? " " + ArgsSufix + "ip " + Ip : "") +
                ((!string.IsNullOrWhiteSpace(Port)) ? " " + ArgsSufix + "port " + Port : "") +
                ((!string.IsNullOrWhiteSpace(HostName)) ? " +hostname \"" + HostName + "\"" : "") +
                base.CommandLine();
        }

    }
}