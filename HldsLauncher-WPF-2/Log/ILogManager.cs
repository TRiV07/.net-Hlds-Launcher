using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Log
{
    public interface ILogManager
    {
        ILogger GetCurrentClassLogger();
        ILogger GetLogger(string name);
    }
}