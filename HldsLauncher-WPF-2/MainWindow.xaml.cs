using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;
using HldsLauncher.Servers;
using HldsLauncher.Log;
using HldsLauncher.Options;
using HldsLauncher.UILogic;
using HldsLauncher.Utils;
using System.Xml.Linq;
using System.Diagnostics;

namespace HldsLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal IServersControl _serversControl;
        internal HldslOptions _options;
        internal bool _expanderCollapsed = false;
        internal string _configFileName = "config.xml";
        //internal string _commandsFileName = "commands.xml";
        internal IDictionary<TabItem, IServer> _tabItems = new Dictionary<TabItem, IServer>();
        internal List<string> autoCompleteBoxConsoleCommandSuggestions = new List<string>();
        internal ContextMenu _contextMenuServersList = new ContextMenu();

        internal System.Windows.Forms.NotifyIcon trayIcon;

        public MainWindow(HldslOptions options)
        {
            InitializeComponent();
            _options = options;
            if (_options == null)
            {
                _options = this.CreateOptions(_configFileName);
            }
            this.InitOptions();
            this.StartupInitializeControls();
            this.StartAllActiveServersAtStartup();
            this.CheckForUpdates();
            InitTrayIcon();
        }

        internal void InitTrayIcon()
        {
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = new System.Drawing.Icon(App.GetResourceStream(new Uri("logo.ico", UriKind.Relative)).Stream);
            trayIcon.Text = Properties.Resources.mw_ti_Text;
            
            trayIcon.Visible = true;
            trayIcon.DoubleClick += (object sender, EventArgs args) =>
                {
                    if (this.WindowState == System.Windows.WindowState.Minimized)
                    {
                        this.Show();
                        this.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                };

        }

        internal void DisposeTrayIcon()
        {
            trayIcon.Dispose();
            trayIcon = null;
        }

        private void dataGridServersList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject depObj = (DependencyObject)e.OriginalSource;

            while (
                (depObj != null) &&
                !(depObj is DataGridCell) &&
                !(depObj is DataGridColumnHeader))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }

            if (depObj == null)
            {
                return;
            }

            if (depObj is DataGridCell)
            {
                while ((depObj != null) && !(depObj is DataGridRow))
                {
                    depObj = VisualTreeHelper.GetParent(depObj);
                }

                DataGridRow dgRow = depObj as DataGridRow;
                dgRow.ContextMenu = _contextMenuServersList;
            }
        }

        private void dataGridServersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject depObj = (DependencyObject)e.OriginalSource;

            while (
                (depObj != null) &&
                !(depObj is DataGridCell) &&
                !(depObj is DataGridColumnHeader))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }

            if (depObj == null)
            {
                return;
            }

            if (depObj is DataGridCell)
            {
                this.OpenProperties();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.CancelOperation();
                if (!Environment.HasShutdownStarted && Environment.ExitCode != 5 && Environment.ExitCode != 10 && Environment.ExitCode != 15)
                {
                    if (MessageBox.Show(Properties.Resources.mw_mb_ExitConfirmation, Properties.Resources.mw_mb_ExitConfirmationTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DisposeTrayIcon();
                        _serversControl.StopAll();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    DisposeTrayIcon();
                    if (Environment.ExitCode == 15)
                    {
                        _serversControl.KillAll();
                        return;
                    }
                    _serversControl.StopAll();
                    if (Environment.ExitCode == 5)
                    {
                        Process.Start(Application.ResourceAssembly.Location);
                    }
                }
            }
            catch
            {
                _serversControl.KillAll();
                if (Environment.ExitCode == 5)
                {
                    Process.Start(Application.ResourceAssembly.Location);
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            this.StartSelectedServer();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            this.StopSelectedServer();
        }

        private void expanderOperations_Expanded(object sender, RoutedEventArgs e)
        {
            if (_expanderCollapsed)
            {
                ((Expander)sender).Width = 200;
                _expanderCollapsed = false;
            }
        }

        private void expanderOperations_Collapsed(object sender, RoutedEventArgs e)
        {
            ((Expander)sender).Width = 25;
            _expanderCollapsed = true;
        }

        private void buttonStartAll_Click(object sender, RoutedEventArgs e)
        {
            this.StartAllActiveServers();
        }

        private void buttonStopAll_Click(object sender, RoutedEventArgs e)
        {
            this.StopAllServers();
        }

        private void buttonCancelOperation_Click(object sender, RoutedEventArgs e)
        {
            this.CancelOperation();
        }

        private void buttonSetTimer_Click(object sender, RoutedEventArgs e)
        {
            this.SetTimerInterval();
        }

        private void buttonRefreshTimer_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshTimerInterval();
        }

        private void buttonMinTimer_Click(object sender, RoutedEventArgs e)
        {
            this.SetMinTimerInterval();
        }

        private void buttonMaxTimer_Click(object sender, RoutedEventArgs e)
        {
            this.SetMaxTimerInterval();
        }

        private void textBoxTimerActual_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationUtil.IsNumber(e.Text);
        }

        private void buttonSiteLink_Click(object sender, RoutedEventArgs e)
        {
            this.GoToAnd1gaming();
        }

        private void buttonSendConsoleCommand_Click(object sender, RoutedEventArgs e)
        {
            this.SendConsoleMessage();
        }

        private void autoCompleteBoxConsoleCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.SendConsoleMessage();
            }
        }

        private void integerUpDownStartInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_options != null)
            {
                _options.StartInterval = (int)e.NewValue;
                this.SaveOptions(_configFileName);
            }
        }

        private void menuItemImportServers_Click(object sender, RoutedEventArgs e)
        {
            this.ImportServers();
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuItemAddGoldSourceServer_Click(object sender, RoutedEventArgs e)
        {
            this.AddGoldSourceServer();
        }

        private void menuItemAddSourceServer_Click(object sender, RoutedEventArgs e)
        {
            this.AddSourceServer();
        }

        private void menuItemAddHltvServer_Click(object sender, RoutedEventArgs e)
        {
            this.AddHltvServer();
        }

        private void menuItemRemoveServer_Click(object sender, RoutedEventArgs e)
        {
            this.RemoveServer();
        }

        private void menuItemServerProperties_Click(object sender, RoutedEventArgs e)
        {
            this.OpenProperties();
        }

        private void menuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            this.ShowOptions();
        }

        internal void textBlockUpdateStatus_MouseUp_Fail(object sender, MouseButtonEventArgs e)
        {
            this.CheckForUpdates();
        }
    }
}
