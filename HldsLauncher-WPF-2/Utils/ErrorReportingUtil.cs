using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;

namespace HldsLauncher.Utils
{
    public static class ErrorReportingUtil
    {
        private static bool _status;
        public static bool Status
        {
            get
            {
                return _status;
            }
            set
            {
                OperatingSystem osInfo = Environment.OSVersion;
                try
                {
                    if (value)
                    {
                        //Включение отчета об ошибках
                        switch (osInfo.Platform)
                        {
                            case PlatformID.Win32NT:
                                {
                                    switch (osInfo.Version.Major)
                                    {
                                        case 5:
                                            {
                                                RegistryKey firstKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                                firstKey.SetValue("DoReport", 1);
                                                firstKey.Close();
                                                RegistryKey secondKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                                secondKey.SetValue("ShowUI", 1);
                                                secondKey.Close();
                                                break;
                                            }
                                        case 6:
                                            {
                                                RegistryKey firstKey = Registry.LocalMachine.CreateSubKey("SYSTEM\\ControlSet001\\Control\\Windows");
                                                firstKey.SetValue("ErrorMode", 0);
                                                firstKey.Close();
                                                RegistryKey secondKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\Windows Error Reporting");
                                                secondKey.SetValue("DontShowUI", 0);
                                                secondKey.Close();
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        //Выключение отчета об ошибках
                        switch (osInfo.Platform)
                        {
                            case PlatformID.Win32NT:
                                {
                                    switch (osInfo.Version.Major)
                                    {
                                        case 5:
                                            {
                                                RegistryKey firstKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                                firstKey.SetValue("DoReport", 0);
                                                firstKey.Close();
                                                RegistryKey secondKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                                secondKey.SetValue("ShowUI", 0);
                                                secondKey.Close();
                                                break;
                                            }
                                        case 6:
                                            {
                                                RegistryKey firstKey = Registry.LocalMachine.CreateSubKey("SYSTEM\\ControlSet001\\Control\\Windows");
                                                firstKey.SetValue("ErrorMode", 2);
                                                firstKey.Close();
                                                RegistryKey secondKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\Windows Error Reporting");
                                                secondKey.SetValue("DontShowUI", 1);
                                                secondKey.Close();
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                    }
                    _status = value;
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show(Properties.Resources.mb_NotEnoughRights, Properties.Resources.mb_NotEnoughRightsTitle);
                }
            }
        }
        
        static ErrorReportingUtil()
        {
            _status = true;
            OperatingSystem osInfo = Environment.OSVersion;
            try
            {
                switch (osInfo.Platform)
                {
                    case PlatformID.Win32NT:
                        {
                            switch (osInfo.Version.Major)
                            {
                                case 5:
                                    {
                                        RegistryKey firstKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                        String ErrorModeString = (firstKey.GetValue("DoReport") != null) ? firstKey.GetValue("DoReport").ToString() : "1";
                                        firstKey.Close();
                                        RegistryKey secondKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PCHealth\\ErrorReporting");
                                        String ShowUIString = (secondKey.GetValue("ShowUI") != null) ? secondKey.GetValue("ShowUI").ToString() : "1";
                                        secondKey.Close();
                                        if (ErrorModeString == "0" && ShowUIString == "0")
                                        {
                                            _status = false;
                                        }
                                        break;
                                    }
                                case 6:
                                    {
                                        RegistryKey firstKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Windows");
                                        String ErrorModeString = (firstKey.GetValue("ErrorMode") != null) ? firstKey.GetValue("ErrorMode").ToString() : "0";
                                        firstKey.Close();
                                        RegistryKey secondKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Windows Error Reporting");
                                        String DontShowUIString = (secondKey.GetValue("DontShowUI") != null) ? secondKey.GetValue("DontShowUI").ToString() : "0";
                                        secondKey.Close();
                                        if (ErrorModeString == "2" && DontShowUIString == "1")
                                        {
                                            _status = false;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(Properties.Resources.mb_NotEnoughRights, Properties.Resources.mb_NotEnoughRightsTitle);
            }
        }
    }
}
