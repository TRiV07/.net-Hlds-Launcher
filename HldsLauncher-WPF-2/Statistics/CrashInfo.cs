using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HldsLauncher.Statistics
{
    public class CrashInfo : INotifyPropertyChanged
    {
        private string _map;
        public string Map { get { return _map; } set { _map = value; OnPropertyChanged("Status"); } }

        private DateTime _time;
        public DateTime Time { get { return _time; } set { _time = value; OnPropertyChanged("Time"); } }

        public CrashInfo()
        {
            Time = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
