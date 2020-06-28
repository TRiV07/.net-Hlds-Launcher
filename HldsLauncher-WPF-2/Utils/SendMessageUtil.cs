using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace HldsLauncher.Utils
{
    public static class SendMessageUtil
    {
        public static bool SendTextMessage(string message, Process process)
        {
            if (message.Length > 0)
            {
                if (process.StartInfo.FileName != "")
                {
                    try
                    {
                        process.WaitForInputIdle(1000);
                        for (int i = 0; i < message.Length; i++)
                        {
                            if (i > 0)
                                if (message[i] == message[i - 1])
                                    Thread.Sleep(5);
                            WinAPI.SendMessage(process.MainWindowHandle, WinAPI.WM_CHAR, message[i], (IntPtr)0);
                        }
                        WinAPI.SendMessage(process.MainWindowHandle, WinAPI.WM_CHAR, (IntPtr)WinAPI.VK_ENTER, (IntPtr)0);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
