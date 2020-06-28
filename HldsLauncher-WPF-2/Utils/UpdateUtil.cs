using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using HldsLauncher.Events;

namespace HldsLauncher.Utils
{
    public static class UpdateUtil
    {
        private const string NO_NEW_VERSION_AVAILABLE = "NO_NEW_VERSION_AVAILABLE";
        private const string NEW_VERSION_AVAILABLE = "NEW_VERSION_AVAILABLE";
        private const string CLIENT_VERSION_FAIL = "CLIENT_VERSION_FAIL";
        private const string INTERNAL_FAIL = "INTERNAL_FAIL";
        private const string OK = "OK";
        
        //private static bool _isCheckingUpdate;
        private static Thread _updateThread;

        static UpdateUtil()
        {

        }

        public static void CheckForUpdate(Action<HldsUpdateEventArgs> completedCallback)
        {
            _updateThread = ThreadUtil.GetThread(() =>
                {
                    string[] response = GetResponseString().Replace("\r", "").Split('\n');
                    if (response[0] == OK)
                    {
                        if (response[1] == NEW_VERSION_AVAILABLE)
                        {
                            completedCallback(new HldsUpdateEventArgs()
                            {
                                UpdateStatus = Enums.HldsUpdateStatus.NewVersionAvailable,
                                NewVersion = response[2],
                                UpdateUrl = response[3]
                            });
                        }
                        else if (response[1] == NO_NEW_VERSION_AVAILABLE)
                        {
                            completedCallback(new HldsUpdateEventArgs()
                            {
                                UpdateStatus = Enums.HldsUpdateStatus.NoNewVersionAvailable
                            });
                        }
                    }
                    else
                    {
                        completedCallback(new HldsUpdateEventArgs()
                            {
                                UpdateStatus = Enums.HldsUpdateStatus.Fail
                            });
                    }
                });
            _updateThread.Start();
        }

        private static string GetResponseString()
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create("http://hldsl.and1gaming.org.ua/Updates/CheckForUpdates?version=" + AssemblyInfoUtil.AssemblyVersion);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        tempString = Encoding.UTF8.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0);
                return sb.ToString();
            }
            catch
            {
                return INTERNAL_FAIL;
            }
        }

        public static void Abort()
        {
            if (_updateThread != null)
            {
                _updateThread.Abort();
            }
        }
    }
}
