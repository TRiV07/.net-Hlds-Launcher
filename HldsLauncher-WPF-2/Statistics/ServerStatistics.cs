using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HldsLauncher.Statistics
{
    public class ServerStatistics : IServerStatistics, INotifyPropertyChanged
    {
        public List<CrashInfo> Crashes { get; set; }

        public ServerStatistics()
        {
            Crashes = new List<CrashInfo>();
        }

        public void AddCrashInfo(string map)
        {
            Crashes.Add(new CrashInfo
            {
                Map = map
            });
            OnPropertyChanged("Crashes");
        }

        public void Reset()
        {
            Crashes.Clear();
            OnPropertyChanged("Crashes");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
