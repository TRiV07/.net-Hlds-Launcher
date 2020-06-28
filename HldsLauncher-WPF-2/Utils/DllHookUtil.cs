using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Utils
{
    public static class DllHookUtil
    {
        public static void CreateHook(string dllPath, ref int hHook, int threadId)
        {
            IntPtr lib = IntPtr.Zero;
            UIntPtr hookFunc = UIntPtr.Zero;
            hHook = 0;
            lib = WinAPI.LoadLibrary(dllPath);
            hookFunc = WinAPI.GetProcAddress(lib, System.Text.ASCIIEncoding.ASCII.GetBytes("_MessageBoxShowProc@12"));
            while (hHook == 0)
            {
                hHook = WinAPI.SetWindowsHookEx((int)WinAPI.HookType.WH_CALLWNDPROC,
                           hookFunc,
                           lib,
                           threadId);
            }
        }

        public static void CloseHook(int hHook)
        {
            WinAPI.UnhookWindowsHookEx(hHook);
        }
    }
}
