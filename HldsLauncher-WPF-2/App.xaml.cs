using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using HldsLauncher.Utils;
using System.IO;
using HldsLauncher.Options;
using System.Globalization;
using System.Management;
using HldsLauncher.ViewModels;

namespace HldsLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private BugReport _bugReport;
        private MainWindow _mainWindow;
        private Thread _mainThread;

        private static MainViewModel viewModel = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        public App()
        {
            //_bugReport = new BugReport();
            _mainThread = Thread.CurrentThread;
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            HldslOptions opts = SetCulture();

            if (Process.GetProcessesByName("HldsLauncher2").Length > 1)
            {
                MessageBox.Show("Приложение уже запущено", "Ошибка запуска");
                this.Shutdown();
            }
            else
            {
                AppDomain.CurrentDomain.AppendPrivatePath("Dlls");

                //AppDomainSetup domainSetup = AppDomain.CurrentDomain.SetupInformation;
                ////domainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                //domainSetup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dlls");

                //AppDomain.CreateDomain("SecondAppDomain", null, domainSetup);
                //AppDomain.Unload(AppDomain.CurrentDomain);

                //AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dlls");

                bool startMinimized = false;
                for (int i = 0; i != e.Args.Length; ++i)
                {
                    if (e.Args[i] == "/StartMinimized")
                    {
                        startMinimized = true;
                    }
                }

                _mainWindow = new MainWindow(opts);
                if (startMinimized)
                {
                    _mainWindow.WindowState = WindowState.Minimized;
                }
                else
                {
                    _mainWindow.Show();
                }
            }
        }

        private HldslOptions SetCulture()
        {
            string fileName = "config.xml";
            string fullPath = string.Format("{0}\\{1}", CommonUtils.GetProgramDirectory(), fileName);
            FileInfo configFile = new FileInfo(fullPath);
            if (configFile.Exists)
            {
                HldslOptions opts = HldslOptions.LoadFromFile(fullPath);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(opts.Language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(opts.Language);
                return opts;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                return null;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //_mainThread.Suspend();
            if (MessageBox.Show(HldsLauncher.Properties.Resources.app_CriticalError, AssemblyInfoUtil.AssemblyProduct, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                BugReport.SendReport((Exception)e.ExceptionObject);
            }
            _mainWindow.DisposeTrayIcon();
            _mainWindow._serversControl.KillAll();

            //ShowReport((Exception)e.ExceptionObject);
        }

        //private int GetParentProcess(int Id)
        //{
        //    int parentPid = 0;
        //    using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
        //    {
        //        mo.Get();
        //        parentPid = Convert.ToInt32(mo["ParentProcessId"]);
        //    }
        //    return parentPid;
        //}

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowReport(e.Exception);
            e.Handled = true;
        }

        private void ShowReport(Exception e)
        {
            //_bugReport.SetException(e);
            BugReport bugReport = new BugReport();
            bugReport.ShowDialog(e);
            switch (bugReport.Result)
            {
                case 5:
                    {
                        Environment.ExitCode = 5;
                        App.Current.Shutdown();
                        break;
                    }
                case 10:
                    {
                        Environment.ExitCode = 10;
                        App.Current.Shutdown();
                        break;
                    }
            }
        }
    }
}
