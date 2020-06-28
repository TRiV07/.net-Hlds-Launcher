using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace HldsLauncher.Utils
{
    public static class AutoStartUtil
    {
        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static void SetAutoStart(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(keyName, assemblyLocation);
        }

        public static void UnSetAutoStart(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            if (key.GetValue(keyName) != null)
                key.DeleteValue(keyName);
        }

        public static bool IsAutoStartEnabled(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false; string value = (string)key.GetValue(keyName);
            if (value == null)
                return false; return (value.Contains(assemblyLocation));
        }

        public static bool IsAutoStartMinimized(string keyName, string minimazedKey)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false; string value = (string)key.GetValue(keyName);
            if (value == null)
                return false; return (value.Contains(minimazedKey));
        }
    }
}
