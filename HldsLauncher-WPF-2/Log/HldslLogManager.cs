using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Log
{
    public static class HldslLogManager
    {
        private static readonly ILogManager _logManager;

        static HldslLogManager()
        {
            _logManager = new NLogManager();
        }

        public static ILogManager GetLogManager()
        {
            return _logManager;
        }

    }
}
