using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using HldsLauncher.Events;

namespace HldsLauncher.Servers
{
    public interface IServersControl
    {
        ObservableCollection<IServer> Servers { get; set; }
        event EventHandler<ServersControlEventArgs> ProgressUpdated;
        void AddFromXml(XNode xServers);
        XNode GetXml();
        void StartAll(int interval);
        void StartAllActive(int interval);
        void DelayedStartAllActive(int interval, int delay);
        void StopAll();
        void KillAll();
        void StopAllInOtherThread();
        void CancelOperation();
    }
}
