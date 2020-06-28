using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HldsLauncher.Servers;
using HldsLauncher.Options;
using HldsLauncher.UILogic;
using System.Threading;
using HldsLauncher.Utils;

namespace HldsLauncher
{
    /// <summary>
    /// Interaction logic for AddEditServerGoldSource.xaml
    /// </summary>
    public partial class AddEditServerGoldSource : Window
    {
        internal IServer server;
        internal GoldSourceServerOptions serverOptions;
        internal MainWindow mainWindow;

        public AddEditServerGoldSource(MainWindow mainWindow, IServer server)
        {
            InitializeComponent();
            this.serverOptions = new GoldSourceServerOptions();
            if (server == null)
            {
                this.server = new GoldSourceServer();
            }
            else
            {
                this.server = server;
                this.server.Options.CloneObjectProps(serverOptions);
            }
            this.InitializeFields();
            this.mainWindow = mainWindow;
            DataContext = serverOptions;
        }

        private void buttonBrowseDsPath_Click(object sender, RoutedEventArgs e)
        {
            AddEditServerLogic.Browse(this, Properties.Resources.aes_GoldSourceServer, "hlds.exe");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
            {
                DialogResult = false;
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            this.Save();
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void comboBoxServerGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddEditServerLogic.InitializeMapList(this, serverOptions.ExecutablePath, false);
        }
    }
}
