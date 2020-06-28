using System.ComponentModel;
namespace HldsLauncher.Servers
{
    public class ValveGameServerStatus : IServerStatus, INotifyPropertyChanged
    {
        private bool _status;
        public bool Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
        private double _fps;
        public double Fps { get { return _fps; } set { _fps = value; OnPropertyChanged("Fps"); } }
        private string _map;
        public string Map { get { return _map; } set { _map = value; OnPropertyChanged("Map"); } }
        private int _maxPlayers;
        public int MaxPlayers { get { return _maxPlayers; } 
            set
            {
                _maxPlayers = value;
                OnPropertyChanged("MaxPlayers");
                _onlineToMaxPlayers = string.Format("{0}/{1}", _onlinePlayers, _maxPlayers);
                OnPropertyChanged("OnlineToMaxPlayers");
            }
        }
        private int _onlinePlayers;
        public int OnlinePlayers { get { return _onlinePlayers; }
            set
            {
                _onlinePlayers = value;
                OnPropertyChanged("OnlinePlayers");
                _onlineToMaxPlayers = string.Format("{0}/{1}", _onlinePlayers, _maxPlayers);
                OnPropertyChanged("OnlineToMaxPlayers");
            }
        }
        private string _onlineToMaxPlayers;
        public string OnlineToMaxPlayers { get { return _onlineToMaxPlayers; } }

        public ValveGameServerStatus()
        {
            Map = "";
            Reset();
        }

        public void Reset()
        {
            Status = false;
            Fps = 0;
            MaxPlayers = 0;
            OnlinePlayers = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}