using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

namespace HldsLauncher.Utils
{
    public static class ThreadUtil
    {
        private static CultureInfo _cultureInfo;
        public static string Culture
        {
            get
            {
                return _cultureInfo.Name;
            }
            set
            {
                _cultureInfo = new CultureInfo(value);
            }
        }

        static ThreadUtil()
        {
            Culture = "en-US";
        }

        public static Thread GetThread(ThreadStart threadStart)
        {
            Thread thread = new Thread(threadStart);
            thread.CurrentCulture = _cultureInfo;
            thread.CurrentUICulture = _cultureInfo;
            return thread;
        }
    }
}
