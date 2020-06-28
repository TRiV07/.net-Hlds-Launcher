using System.Threading;
using HldsLauncher.Log;

namespace HldsLauncher.Utils
{
    public static class NtTimerResolutionUtil
    {
        //private static ILogger _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();

        private static int _min;
        public static int Min { get { return _min; } }

        private static int _max;
        public static int Max { get { return _max; } }

        private static int _actual;
        public static int Actual { get { return _actual; } }

        private static Thread timerSetterThread;
        
        static NtTimerResolutionUtil()
        {
            WinAPI.NtQueryTimerResolution(out _max, out _min, out _actual);
            timerSetterThread = ThreadUtil.GetThread(() =>
                {
                    while (true)
                    {
                        int newResolution = _actual;
                        WinAPI.NtSetTimerResolution(newResolution, true, out _actual);
                        Thread.Sleep(1000);
                    }
                });
            timerSetterThread.IsBackground = true;
            timerSetterThread.Start();
        }

        public static int SetResolution(int newResolution)
        {
#if DEBUG 
            //_logger.Debug(string.Format("Setting new timmer value ({0})", newResolution));
#endif
            WinAPI.NtSetTimerResolution(newResolution, true, out _actual);
#if DEBUG
            //_logger.Debug(string.Format("New resolution is ({0})", _actual));
#endif
            return _actual;
        }

        public static void RefreshResolution()
        {
            WinAPI.NtQueryTimerResolution(out _max, out _min, out _actual);
        }
    }
}
