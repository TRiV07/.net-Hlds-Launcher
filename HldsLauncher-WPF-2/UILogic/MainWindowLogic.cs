using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using HldsLauncher.Events;
using HldsLauncher.Options;
using HldsLauncher.Servers;
using HldsLauncher.Utils;
using System.Reflection;
using HldsLauncher.Enums;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using HldsLauncher.Log;
using System.Threading;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;

namespace HldsLauncher.UILogic
{
    public static class MainWindowLogic
    {
        private static ILogger _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();

        public static void AddServersForTests(this MainWindow mainWindow)
        {
            mainWindow._serversControl.Servers.Add(new GoldSourceServer());
            mainWindow._serversControl.Servers[0].Options.ExecutablePath = @"c:\Tmp\cstrike_win_new\Counter Strike 1.6\hlds.exe";

            mainWindow._serversControl.Servers.Add(new GoldSourceServer());
            mainWindow._serversControl.Servers[1].Options.ExecutablePath = @"c:\Tmp\cstrike_win_new\Counter Strike 1.6\hlds.exe";
            (mainWindow._serversControl.Servers[1].Options as GoldSourceServerOptions).Port = "27016";

            mainWindow._serversControl.Servers.Add(new GoldSourceServer());
            mainWindow._serversControl.Servers[2].Options.ExecutablePath = @"c:\Tmp\cstrike_win_new\Counter Strike 1.6\hlds.exe";
            (mainWindow._serversControl.Servers[2].Options as GoldSourceServerOptions).Port = "27017";

            mainWindow._serversControl.Servers.Add(new GoldSourceServer());
            mainWindow._serversControl.Servers[3].Options.ExecutablePath = @"c:\Tmp\cstrike_win_new\Counter Strike 1.6\hlds.exe";
            (mainWindow._serversControl.Servers[3].Options as GoldSourceServerOptions).Port = "27018";
        }

        public static HldslOptions CreateOptions(this MainWindow mainWindow, string fileName)
        {
            string fullPath = string.Format("{0}\\{1}", CommonUtils.GetProgramDirectory(), fileName);
            FileInfo configFile = new FileInfo(fullPath);

            HldslOptions opts = new HldslOptions();
            if (Languages.LanguagesList.FirstOrDefault(x => x.Value == Thread.CurrentThread.CurrentCulture.Name) != null)
            {
                opts.Language = Thread.CurrentThread.CurrentCulture.Name;
                ThreadUtil.Culture = opts.Language;
            }
            else
            {
                opts.Language = "en-US";
            }
            opts.SaveToFile(fullPath);
            return opts;
        }

        public static void InitOptions(this MainWindow mainWindow)
        {
            ThreadUtil.Culture = mainWindow._options.Language;
            mainWindow._serversControl = new ServersControl();
            mainWindow._serversControl.AddFromXml(mainWindow._options.Servers);
        }

        public static void SaveOptions(this MainWindow mainWindow, string fileName)
        {
            string fullPath = string.Format("{0}\\{1}", CommonUtils.GetProgramDirectory(), fileName);
            FileInfo serversFile = new FileInfo(fullPath);
            mainWindow._options.Servers = (XElement)mainWindow._serversControl.GetXml();
            mainWindow._options.SaveToFile(fullPath);
        }

        public static void StartupInitializeControls(this MainWindow mainWindow)
        {
            mainWindow.Title = String.Format("{0} {1}", AssemblyInfoUtil.AssemblyProduct, AssemblyInfoUtil.AssemblyVersion);
            mainWindow.taskBarItemInfoHlds.Description = String.Format("{0} {1}", AssemblyInfoUtil.AssemblyProduct, AssemblyInfoUtil.AssemblyVersion);

            RefreshTimerInterval(mainWindow);
            if (mainWindow._options.MMTimerStartUpSet)
            {
                _logger.Info(string.Format(Properties.Resources.log_StartupTimerSet, mainWindow._options.NtTimerResolution));
                SetTimerInterval(mainWindow, mainWindow._options.NtTimerResolution);
            }
            mainWindow.integerUpDownStartInterval.Value = mainWindow._options.StartInterval;
            mainWindow._serversControl.ProgressUpdated += (object sender, Events.ServersControlEventArgs e) =>
            {
                mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (e.ServersCount == e.ServersDone)
                    {
                        EnableActionsControls(mainWindow);
                        mainWindow.taskBarItemInfoHlds.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                    }
                    else
                    {
                        mainWindow.taskBarItemInfoHlds.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        mainWindow.taskBarItemInfoHlds.ProgressValue = (double)e.ServersDone / (double)e.ServersCount;
                        mainWindow.progressBarStartStopOperation.Maximum = e.ServersCount;
                        mainWindow.progressBarStartStopOperation.Value = e.ServersDone;
                    }
                }));
            };

            foreach (IServer server in mainWindow._serversControl.Servers)
            {
                if (server.Options.Type != ServerType.Hltv)
                {
                    CreateControlsForServer(mainWindow, server);
                }
            }

            mainWindow.richTextBoxLog.Document.Blocks.Clear();
            mainWindow.dataGridServersList.ItemsSource = mainWindow._serversControl.Servers;
            mainWindow.integerUpDownStartInterval.Value = mainWindow._options.StartInterval;

            InitializeDataGridServersListContextMenu(mainWindow, mainWindow._contextMenuServersList);
            foreach (XElement command in mainWindow._options.Commands.XPathSelectElements("Command"))
            {
                mainWindow.autoCompleteBoxConsoleCommandSuggestions.Add(command.Value);
            }
            ApplyDataBinding(mainWindow);
        }

        public static void CreateControlsForServer(this MainWindow mainWindow, IServer server)
        {
            bool mouseDown = false;
            ////RichTextBox creation
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.SetValue(Paragraph.LineHeightProperty, 0.1);
            richTextBox.FontFamily = new FontFamily("Courier New");
            richTextBox.FontSize = 12;
            richTextBox.FontWeight = FontWeights.Bold;
            richTextBox.AutoWordSelection = false;
            richTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            richTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            richTextBox.IsReadOnly = true;
            richTextBox.Document.PageWidth = 588;
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run("")));
            richTextBox.IsUndoEnabled = false;
            richTextBox.UndoLimit = 0;
            richTextBox.PreviewMouseDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                {
                    mouseDown = true;
                };
            richTextBox.PreviewMouseUp += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                {
                    ((RichTextBox)sender).Copy();
                    mouseDown = false;
                };
            richTextBox.MouseLeave += (object sender, System.Windows.Input.MouseEventArgs e) =>
                {
                    //((RichTextBox)sender).Selection.Select(((RichTextBox)sender).CaretPosition, ((RichTextBox)sender).CaretPosition);
                    mouseDown = false;
                };

            server.ConsoleUpdated += (object sender, ConsoleEventArgs e) =>
                {
                    if (!mouseDown)
                    {
                        mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                            //tr.Text = e2.Console.Substring(0, 2500);

                            //FlowDocument myFlowDoc = new FlowDocument();
                            //myFlowDoc.Blocks.Add(new Paragraph(new Run(e.Console)));
                            //richTextBox.Document = myFlowDoc;

                            //richTextBox.Document.Blocks.Clear();
                            //(richTextBox.Document.Blocks.FirstBlock as Paragraph).Inlines.Clear();
                            ((richTextBox.Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text = e.Console.TrimEnd();
                            //richTextBox.Document.Blocks.Add(new Paragraph(new Run(e.Console)));

                        }),
                        System.Windows.Threading.DispatcherPriority.Render);
                    }

                };

            //TabItem creation
            TabItem tabItem = new TabItem();
            if (mainWindow._options.ShowTabsOnlyIfServerStarted)
            {
                tabItem.Visibility = Visibility.Collapsed;
            }
            tabItem.Content = richTextBox;
            tabItem.Header = server.Options.HostName.Replace("_", "__");

            server.ServerInfoUpdated += (object sender, ServerInfoEventArgs e) =>
                {
                    mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (mainWindow._options.ShowServerInfoInTabs)
                        {
                            tabItem.Header = string.Format("{0} ({1}) ({2})", server.Options.HostName.Replace("_", "__"), e.ServerStatus.OnlineToMaxPlayers, e.ServerStatus.Map.Replace("_", "__"));
                        }
                        else
                        {
                            tabItem.Header = server.Options.HostName.Replace("_", "__");
                        }
                        int onlinePlayers = 0;
                        int maxPlayers = 0;
                        foreach (IServer serverC in mainWindow._serversControl.Servers)
                        {
                            onlinePlayers += serverC.ServerStatus.OnlinePlayers;
                            maxPlayers += serverC.ServerStatus.MaxPlayers;
                        }
                        mainWindow.textBlockPlayersCount.Text = string.Format(Properties.Resources.mw_AllPlayersCount, onlinePlayers, maxPlayers);
                    }),
                    System.Windows.Threading.DispatcherPriority.Render);
                };

            server.ServerStoped += (object sender, EventArgs e) =>
                {
                    mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tabItem.Header = server.Options.HostName.Replace("_", "__");
                        int onlinePlayers = 0;
                        int maxPlayers = 0;
                        foreach (IServer serverC in mainWindow._serversControl.Servers)
                        {
                            onlinePlayers += serverC.ServerStatus.OnlinePlayers;
                            maxPlayers += serverC.ServerStatus.MaxPlayers;
                        }
                        mainWindow.textBlockPlayersCount.Text = string.Format(Properties.Resources.mw_AllPlayersCount, onlinePlayers, maxPlayers);
                        if (mainWindow._options.ShowTabsOnlyIfServerStarted)
                        {
                            tabItem.Visibility = Visibility.Collapsed;
                        }
                    }),
                    System.Windows.Threading.DispatcherPriority.Render);
                };

            server.ServerStarted += (object sender, EventArgs e) =>
            {
                mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    tabItem.Visibility = Visibility.Visible;
                }),
                System.Windows.Threading.DispatcherPriority.Render);
            };

            mainWindow.tabControlServersLogs.Items.Add(tabItem);
            mainWindow._tabItems.Add(new System.Collections.Generic.KeyValuePair<TabItem, IServer>(tabItem, server));
        }

        public static void RemoveControlsForServer(this MainWindow mainWindow, IServer server)
        {
            TabItem tabItem = null;
            foreach (var keyPair in mainWindow._tabItems)
            {
                if (keyPair.Value == server)
                {
                    tabItem = keyPair.Key;
                    continue;
                }
            }
            if (tabItem != null)
            {
                mainWindow.tabControlServersLogs.Items.Remove(tabItem);
                mainWindow._tabItems.Remove(tabItem);
            }
        }

        public static void ImportServers(this MainWindow mainWindow)
        {
            string oldConfigFileName = "servers.xml";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = oldConfigFileName;
            dialog.Filter = string.Format("{0}|{1}", Properties.Resources.mw_OldHldslOptionsFile, oldConfigFileName);
            if (dialog.ShowDialog() == true)
            {
                XElement oldConfig = XElement.Load(dialog.FileName);
                int serversAdded = 0;
                foreach (XElement xServer in oldConfig.XPathSelectElements("Servers/Server"))
                {
                    IServer server = null;
                    switch (xServer.XPathSelectElement("Game").Value)
                    {
                        case "cstrike":
                            {
                                server = new GoldSourceServer();
                                (server.Options as GoldSourceServerOptions).Game = GoldSourceGame.cstrike;

                                if (xServer.XPathSelectElement("Ip") != null)
                                    (server.Options as GoldSourceServerOptions).Ip = xServer.XPathSelectElement("Ip").Value;
                                if (xServer.XPathSelectElement("Port") != null)
                                    (server.Options as GoldSourceServerOptions).Port = xServer.XPathSelectElement("Port").Value;
                                if (xServer.XPathSelectElement("Map") != null)
                                    (server.Options as GoldSourceServerOptions).Map = xServer.XPathSelectElement("Map").Value;
                                if (xServer.XPathSelectElement("MaxPlayers") != null)
                                    (server.Options as GoldSourceServerOptions).MaxPlayers = int.Parse(xServer.XPathSelectElement("MaxPlayers").Value);
                                if (xServer.XPathSelectElement("RconPassword") != null)
                                    (server.Options as GoldSourceServerOptions).RconPassword = xServer.XPathSelectElement("RconPassword").Value;
                                if (xServer.XPathSelectElement("HostName") != null)
                                    (server.Options as GoldSourceServerOptions).HostName = xServer.XPathSelectElement("HostName").Value;
                                if (xServer.XPathSelectElement("DopArgs") != null)
                                    (server.Options as GoldSourceServerOptions).AdditionalCommandLineArgs = xServer.XPathSelectElement("DopArgs").Value;
                                if (xServer.XPathSelectElement("HldsExe") != null)
                                    (server.Options as GoldSourceServerOptions).ExecutablePath = xServer.XPathSelectElement("HldsExe").Value;
                                if (xServer.XPathSelectElement("ConsolePosX") != null)
                                    (server.Options as GoldSourceServerOptions).ConsolePositionX = int.Parse(xServer.XPathSelectElement("ConsolePosX").Value);
                                if (xServer.XPathSelectElement("ConsolePosY") != null)
                                    (server.Options as GoldSourceServerOptions).ConsolePositionY = int.Parse(xServer.XPathSelectElement("ConsolePosY").Value);
                                if (xServer.XPathSelectElement("AutoRestart") != null)
                                    (server.Options as GoldSourceServerOptions).AutoRestart = bool.Parse(xServer.XPathSelectElement("AutoRestart").Value);
                                if (xServer.XPathSelectElement("ActiveServer") != null)
                                    (server.Options as GoldSourceServerOptions).ActiveServer = bool.Parse(xServer.XPathSelectElement("ActiveServer").Value);
                                if (xServer.XPathSelectElement("IntegratedConsole") != null)
                                    (server.Options as GoldSourceServerOptions).ConsoleType = (bool.Parse(xServer.XPathSelectElement("IntegratedConsole").Value) ? ConsoleType.Integrated : ConsoleType.Native);

                                break;
                            }
                        case "cstrikesource":
                            {
                                server = new SourceServer();
                                (server.Options as SourceServerOptions).Game = SourceGame.cstrike;

                                if (xServer.XPathSelectElement("Ip") != null)
                                    (server.Options as SourceServerOptions).Ip = xServer.XPathSelectElement("Ip").Value;
                                if (xServer.XPathSelectElement("Port") != null)
                                    (server.Options as SourceServerOptions).Port = xServer.XPathSelectElement("Port").Value;
                                if (xServer.XPathSelectElement("Map") != null)
                                    (server.Options as SourceServerOptions).Map = xServer.XPathSelectElement("Map").Value;
                                if (xServer.XPathSelectElement("MaxPlayers") != null)
                                    (server.Options as SourceServerOptions).MaxPlayers = int.Parse(xServer.XPathSelectElement("MaxPlayers").Value);
                                if (xServer.XPathSelectElement("RconPassword") != null)
                                    (server.Options as SourceServerOptions).RconPassword = xServer.XPathSelectElement("RconPassword").Value;
                                if (xServer.XPathSelectElement("HostName") != null)
                                    (server.Options as SourceServerOptions).HostName = xServer.XPathSelectElement("HostName").Value;
                                if (xServer.XPathSelectElement("DopArgs") != null)
                                    (server.Options as SourceServerOptions).AdditionalCommandLineArgs = xServer.XPathSelectElement("DopArgs").Value;
                                if (xServer.XPathSelectElement("HldsExe") != null)
                                    (server.Options as SourceServerOptions).ExecutablePath = xServer.XPathSelectElement("HldsExe").Value;
                                if (xServer.XPathSelectElement("ConsolePosX") != null)
                                    (server.Options as SourceServerOptions).ConsolePositionX = int.Parse(xServer.XPathSelectElement("ConsolePosX").Value);
                                if (xServer.XPathSelectElement("ConsolePosY") != null)
                                    (server.Options as SourceServerOptions).ConsolePositionY = int.Parse(xServer.XPathSelectElement("ConsolePosY").Value);
                                if (xServer.XPathSelectElement("AutoRestart") != null)
                                    (server.Options as SourceServerOptions).AutoRestart = bool.Parse(xServer.XPathSelectElement("AutoRestart").Value);
                                if (xServer.XPathSelectElement("ActiveServer") != null)
                                    (server.Options as SourceServerOptions).ActiveServer = bool.Parse(xServer.XPathSelectElement("ActiveServer").Value);
                                if (xServer.XPathSelectElement("IntegratedConsole") != null)
                                    (server.Options as SourceServerOptions).ConsoleType = (bool.Parse(xServer.XPathSelectElement("IntegratedConsole").Value) ? ConsoleType.Integrated : ConsoleType.Native);

                                break;
                            }
                        case "hltv":
                            {
                                server = new ValveHltvServer();

                                if (xServer.XPathSelectElement("Ip") != null)
                                    (server.Options as ValveHltvServerOptions).Ip = xServer.XPathSelectElement("Ip").Value;
                                if (xServer.XPathSelectElement("Port") != null)
                                    (server.Options as ValveHltvServerOptions).Port = xServer.XPathSelectElement("Port").Value;
                                if (xServer.XPathSelectElement("Map") != null)
                                    (server.Options as ValveHltvServerOptions).HostName = xServer.XPathSelectElement("HostName").Value;
                                if (xServer.XPathSelectElement("DopArgs") != null)
                                    (server.Options as ValveHltvServerOptions).AdditionalCommandLineArgs = xServer.XPathSelectElement("DopArgs").Value;
                                if (xServer.XPathSelectElement("HldsExe") != null)
                                    (server.Options as ValveHltvServerOptions).ExecutablePath = xServer.XPathSelectElement("HldsExe").Value;
                                if (xServer.XPathSelectElement("ConsolePosX") != null)
                                    (server.Options as ValveHltvServerOptions).ConsolePositionX = int.Parse(xServer.XPathSelectElement("ConsolePosX").Value);
                                if (xServer.XPathSelectElement("ConsolePosY") != null)
                                    (server.Options as ValveHltvServerOptions).ConsolePositionY = int.Parse(xServer.XPathSelectElement("ConsolePosY").Value);
                                if (xServer.XPathSelectElement("AutoRestart") != null)
                                    (server.Options as ValveHltvServerOptions).AutoRestart = bool.Parse(xServer.XPathSelectElement("AutoRestart").Value);
                                if (xServer.XPathSelectElement("ActiveServer") != null)
                                    (server.Options as ValveHltvServerOptions).ActiveServer = bool.Parse(xServer.XPathSelectElement("ActiveServer").Value);

                                break;
                            }
                    }
                    if (server != null)
                    {
                        mainWindow._serversControl.Servers.Add(server);
                        CreateControlsForServer(mainWindow, server);
                        serversAdded++;
                        _logger.Info(string.Format(Properties.Resources.mw_ServerImported, server.Options.HostName));
                    }
                }
                if (serversAdded > 0)
                {
                    SaveOptions(mainWindow, mainWindow._configFileName);
                }
            }
        }

        public static void AddGoldSourceServer(this MainWindow mainWindow)
        {
            IServer server = new GoldSourceServer();
            AddEditServerGoldSource editWindow = new AddEditServerGoldSource(mainWindow, server);
            editWindow.Title = Properties.Resources.aes_AddNewServerTitle;
            if (editWindow.ShowDialog() == true)
            {
                mainWindow._serversControl.Servers.Add(server);
                CreateControlsForServer(mainWindow, server);
                SaveOptions(mainWindow, mainWindow._configFileName);
            }
        }

        public static void AddSourceServer(this MainWindow mainWindow)
        {
            IServer server = new SourceServer();
            AddEditServerSource editWindow = new AddEditServerSource(mainWindow, server);
            editWindow.Title = Properties.Resources.aes_AddNewServerTitle;
            if (editWindow.ShowDialog() == true)
            {
                mainWindow._serversControl.Servers.Add(server);
                CreateControlsForServer(mainWindow, server);
                SaveOptions(mainWindow, mainWindow._configFileName);
            }
        }

        public static void AddHltvServer(this MainWindow mainWindow)
        {
            IServer server = new ValveHltvServer();
            AddEditServerHltv editWindow = new AddEditServerHltv(mainWindow, server);
            editWindow.Title = Properties.Resources.aes_AddNewServerTitle;
            if (editWindow.ShowDialog() == true)
            {
                mainWindow._serversControl.Servers.Add(server);
                SaveOptions(mainWindow, mainWindow._configFileName);
            }
        }

        public static void RemoveServer(this MainWindow mainWindow)
        {
            if (mainWindow.dataGridServersList.SelectedCells.Count > 0)
                if (mainWindow.dataGridServersList.SelectedIndex >= 0)
                {
                    IServer server = (mainWindow.dataGridServersList.SelectedItem as IServer);
                    if (MessageBox.Show(
                        string.Format(Properties.Resources.mw_mb_RemoveConfirmation, server.Options.HostName),
                        Properties.Resources.mw_mb_RemoveConfirmationTitle,
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        server.Stop();
                        RemoveControlsForServer(mainWindow, server);
                        mainWindow._serversControl.Servers.Remove(server);
                        SaveOptions(mainWindow, mainWindow._configFileName);
                    }
                }
        }

        public static void InitializeDataGridServersListContextMenu(this MainWindow mainWindow, ContextMenu contextMenuServersList)
        {
            contextMenuServersList.Opened += (object sender, RoutedEventArgs e) =>
            {
                if (mainWindow.dataGridServersList.SelectedCells.Count > 0)
                    if (mainWindow.dataGridServersList.SelectedIndex >= 0)
                    {
                        ((MenuItem)contextMenuServersList.Items[0]).Header =
                            (mainWindow.dataGridServersList.SelectedItem as IServer).ServerStatus.Status ?
                                Properties.Resources.mw_cm_StopServer :
                                Properties.Resources.mw_cm_StartServer;
                    }
            };

            contextMenuServersList.Items.Add(
                new MenuItem
                {
                    Name = "contextMenuItemServerStartStop",
                    Header = Properties.Resources.mw_cm_StartServer
                }
            );
            ((MenuItem)contextMenuServersList.Items[0]).Click += (object sender, RoutedEventArgs e) =>
            {
                if (mainWindow.dataGridServersList.SelectedCells.Count > 0)
                    if (mainWindow.dataGridServersList.SelectedIndex >= 0)
                    {
                        IServer server = (mainWindow.dataGridServersList.SelectedItem as IServer);
                        if (server.ServerStatus.Status)
                            StopServer(mainWindow, server);
                        else
                            StartServer(mainWindow, server);
                    }

            };
            contextMenuServersList.Items.Add(
                new MenuItem
                {
                    Name = "contextMenuItemRemoveServer",
                    Header = Properties.Resources.mw_cm_RemoveServer
                }
            );
            ((MenuItem)contextMenuServersList.Items[1]).Click += (object sender, RoutedEventArgs e) =>
            {
                RemoveServer(mainWindow);
            };
            contextMenuServersList.Items.Add(
                new MenuItem
                {
                    Name = "contextMenuItemServerOptions",
                    Header = Properties.Resources.mw_cm_Properties
                }
            );
            ((MenuItem)contextMenuServersList.Items[2]).Click += (object sender, RoutedEventArgs e) =>
            {
                OpenProperties(mainWindow);
            };
        }

        public static void OpenProperties(this MainWindow mainWindow)
        {
            if (mainWindow.dataGridServersList.SelectedCells.Count > 0)
                if (mainWindow.dataGridServersList.SelectedIndex >= 0)
                {
                    IServer server = (mainWindow.dataGridServersList.SelectedItem as IServer);
                    switch (server.Options.Type)
                    {
                        case ServerType.GoldSource:
                            {
                                AddEditServerGoldSource editWindow = new AddEditServerGoldSource(mainWindow, server);
                                editWindow.Title = string.Format(Properties.Resources.aes_EditServerTitle, server.Options.HostName);
                                if (editWindow.ShowDialog() == true)
                                {
                                    SaveOptions(mainWindow, mainWindow._configFileName);
                                }
                                break;
                            }
                        case ServerType.Source:
                            {
                                AddEditServerSource editWindow = new AddEditServerSource(mainWindow, server);
                                editWindow.Title = string.Format(Properties.Resources.aes_EditServerTitle, server.Options.HostName);
                                if (editWindow.ShowDialog() == true)
                                {
                                    SaveOptions(mainWindow, mainWindow._configFileName);
                                }
                                break;
                            }
                        case ServerType.Hltv:
                            {
                                AddEditServerHltv editWindow = new AddEditServerHltv(mainWindow, server);
                                editWindow.Title = string.Format(Properties.Resources.aes_EditServerTitle, server.Options.HostName);
                                if (editWindow.ShowDialog() == true)
                                {
                                    SaveOptions(mainWindow, mainWindow._configFileName);
                                }
                                break;
                            }
                    }
                    mainWindow.dataGridServersList.Items.Refresh();
                }
        }

        public static void StartSelectedServer(this MainWindow mainWindow)
        {
            if (mainWindow.dataGridServersList.SelectedItem as IServer != null)
                (mainWindow.dataGridServersList.SelectedItem as IServer).Start();
        }

        public static void StopSelectedServer(this MainWindow mainWindow)
        {
            if (mainWindow.dataGridServersList.SelectedItem as IServer != null)
                (mainWindow.dataGridServersList.SelectedItem as IServer).Stop();
        }

        public static void StartServer(this MainWindow mainWindow, IServer server)
        {
            server.Start();
        }

        public static void StopServer(this MainWindow mainWindow, IServer server)
        {
            server.Stop();
        }

        public static void StartAllServers(this MainWindow mainWindow)
        {
            DisableActionsControls(mainWindow);
            mainWindow._serversControl.StartAll(
                (mainWindow.integerUpDownStartInterval.Value.HasValue ? mainWindow.integerUpDownStartInterval.Value.Value : 3) * 1000
            );
        }

        public static void StartAllActiveServers(this MainWindow mainWindow)
        {
            DisableActionsControls(mainWindow);
            mainWindow._serversControl.StartAllActive(
                (mainWindow.integerUpDownStartInterval.Value.HasValue ? mainWindow.integerUpDownStartInterval.Value.Value : 3) * 1000
            );
        }

        public static void StartAllActiveServersAtStartup(this MainWindow mainWindow)
        {
            if (mainWindow._options.StartServersAfterProgramStart)
            {
                DisableActionsControls(mainWindow);
                mainWindow.textBlockStatus.Text = Properties.Resources.mw_StatusWaitingForServersStart;
                mainWindow.taskBarItemInfoHlds.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                mainWindow._serversControl.DelayedStartAllActive(
                    (mainWindow._options.StartInterval) * 1000, (mainWindow._options.WaitBeforeServersStart) * 1000
                );
            }
        }

        public static void StopAllServers(this MainWindow mainWindow)
        {

            mainWindow._serversControl.StopAllInOtherThread();
        }

        public static void EnableActionsControls(this MainWindow mainWindow)
        {
            mainWindow.progressBarStartStopOperation.Value = 0;
            mainWindow.taskBarItemInfoHlds.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            mainWindow.buttonCancelOperation.IsEnabled = false;
            mainWindow.buttonStartAll.IsEnabled = true;
            mainWindow.buttonStopAll.IsEnabled = true;
            mainWindow.buttonStart.IsEnabled = true;
            mainWindow.buttonStop.IsEnabled = true;
            mainWindow.textBlockStatus.Text = "";
        }

        public static void DisableActionsControls(this MainWindow mainWindow)
        {
            mainWindow.buttonCancelOperation.IsEnabled = true;
            mainWindow.buttonStartAll.IsEnabled = false;
            mainWindow.buttonStopAll.IsEnabled = false;
            mainWindow.buttonStart.IsEnabled = false;
            mainWindow.buttonStop.IsEnabled = false;
        }

        public static void CancelOperation(this MainWindow mainWindow)
        {
            mainWindow._serversControl.CancelOperation();
            EnableActionsControls(mainWindow);
        }

        public static void RefreshTimerInterval(this MainWindow mainWindow)
        {
            NtTimerResolutionUtil.RefreshResolution();
            mainWindow.textBoxTimeMax.Text = NtTimerResolutionUtil.Max.ToString();
            mainWindow.textBoxTimerMin.Text = NtTimerResolutionUtil.Min.ToString();
            mainWindow.textBoxTimerActual.Text = NtTimerResolutionUtil.Actual.ToString();
            _logger.Info(string.Format(Properties.Resources.log_TimerIntervalMin, mainWindow.textBoxTimeMax.Text));
            _logger.Info(string.Format(Properties.Resources.log_TimerIntervalMax, mainWindow.textBoxTimerMin.Text));
            _logger.Info(string.Format(Properties.Resources.log_TimerIntervalActual,  mainWindow.textBoxTimerActual.Text));
        }

        public static void SetTimerInterval(this MainWindow mainWindow)
        {
            if (mainWindow.textBoxTimerActual.Text.Length > 0)
            {
#if DEBUG
                _logger.Debug("Setting value: " + mainWindow.textBoxTimerActual.Text);
#endif
                NtTimerResolutionUtil.SetResolution(int.Parse(mainWindow.textBoxTimerActual.Text));
#if DEBUG
                _logger.Debug("New value: " + NtTimerResolutionUtil.Actual.ToString());
#endif
            }
            RefreshTimerInterval(mainWindow);
            mainWindow._options.NtTimerResolution = NtTimerResolutionUtil.Actual;
            SaveOptions(mainWindow, mainWindow._configFileName);
        }

        public static void SetTimerInterval(this MainWindow mainWindow, int newInterval)
        {
            NtTimerResolutionUtil.SetResolution(newInterval);
            mainWindow._options.NtTimerResolution = NtTimerResolutionUtil.Actual;
            mainWindow.textBoxTimerActual.Text = NtTimerResolutionUtil.Actual.ToString();
            SaveOptions(mainWindow, mainWindow._configFileName);
        }

        public static void SetMinTimerInterval(this MainWindow mainWindow)
        {
            mainWindow.textBoxTimerActual.Text = "";
#if DEBUG
            _logger.Debug("Setting min: " + NtTimerResolutionUtil.Min.ToString());
#endif
            NtTimerResolutionUtil.SetResolution(NtTimerResolutionUtil.Min);
#if DEBUG
            _logger.Debug("New value: " + NtTimerResolutionUtil.Actual.ToString());
#endif

            RefreshTimerInterval(mainWindow);
            mainWindow._options.NtTimerResolution = NtTimerResolutionUtil.Actual;
            SaveOptions(mainWindow, mainWindow._configFileName);
        }

        public static void SetMaxTimerInterval(this MainWindow mainWindow)
        {
            mainWindow.textBoxTimerActual.Text = "";
#if DEBUG
            _logger.Debug("Setting max: " + NtTimerResolutionUtil.Max.ToString());
#endif
            NtTimerResolutionUtil.SetResolution(int.Parse(NtTimerResolutionUtil.Max.ToString()));
#if DEBUG
            _logger.Debug("New value: " + NtTimerResolutionUtil.Actual.ToString());
#endif
            RefreshTimerInterval(mainWindow);
            mainWindow._options.NtTimerResolution = NtTimerResolutionUtil.Actual;
            SaveOptions(mainWindow, mainWindow._configFileName);
        }

        public static void GoToAnd1gaming(this MainWindow mainWindow)
        {
            System.Diagnostics.Process.Start("http://forum.and1gaming.org.ua/topic/88/");
        }

        public static void ShowOptions(this MainWindow mainWindow)
        {
            DialogOptions dialogOptions = new DialogOptions(mainWindow._options);
            if (dialogOptions.ShowDialog() == true)
            {
                SaveOptions(mainWindow, mainWindow._configFileName);
                if (mainWindow._options.ShowTabsOnlyIfServerStarted)
                {
                    foreach (TabItem tabItem in mainWindow._tabItems.Keys)
                    {
                        if (!mainWindow._tabItems[tabItem].ServerStatus.Status)
                        {
                            tabItem.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else
                {
                    foreach (TabItem tabItem in mainWindow._tabItems.Keys)
                    {
                        tabItem.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private static MouseButtonEventHandler textBlockUpdateStatus_MouseUp_Recheck;
        private static MouseButtonEventHandler textBlockUpdateStatus_MouseUp_GetNewVersion;

        public static void CheckForUpdates(this MainWindow mainWindow)
        {
            try
            {
                mainWindow.textBlockUpdateStatus.MouseUp -= textBlockUpdateStatus_MouseUp_Recheck;
            }
            catch { }
            try
            {
                mainWindow.textBlockUpdateStatus.MouseUp -= textBlockUpdateStatus_MouseUp_GetNewVersion;
            }
            catch { }
            mainWindow.textBlockUpdateStatus.Text = Properties.Resources.mw_upd_CheckingNewVersion;
            mainWindow.textBlockUpdateStatus.Foreground = Brushes.Black;
            UpdateUtil.CheckForUpdate((HldsUpdateEventArgs e) =>
                {
                    mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            switch (e.UpdateStatus)
                            {
                                case HldsUpdateStatus.Fail:
                                    {
                                        mainWindow.textBlockUpdateStatus.Text = Properties.Resources.mw_upd_FailCheckForUpdate;
                                        textBlockUpdateStatus_MouseUp_Recheck = (object sender, MouseButtonEventArgs e1) =>
                                            {
                                                CheckForUpdates(mainWindow);
                                            };
                                        mainWindow.textBlockUpdateStatus.MouseUp += textBlockUpdateStatus_MouseUp_Recheck;
                                        break;
                                    }
                                case HldsUpdateStatus.NewVersionAvailable:
                                    {
                                        mainWindow.textBlockUpdateStatus.Text = string.Format(Properties.Resources.mw_upd_NewVersionAvailable, e.NewVersion);
                                        mainWindow.textBlockUpdateStatus.Foreground = Brushes.Red;
                                        textBlockUpdateStatus_MouseUp_GetNewVersion = (object sender, MouseButtonEventArgs e1) =>
                                        {
                                            System.Diagnostics.Process.Start(e.UpdateUrl);
                                        };
                                        mainWindow.textBlockUpdateStatus.MouseUp += textBlockUpdateStatus_MouseUp_GetNewVersion;
                                        break;
                                    }
                                case HldsUpdateStatus.NoNewVersionAvailable:
                                    {
                                        mainWindow.textBlockUpdateStatus.Text = string.Format(Properties.Resources.mw_upd_NoNewVersionAvailable, e.NewVersion);
                                        textBlockUpdateStatus_MouseUp_Recheck = (object sender, MouseButtonEventArgs e1) =>
                                        {
                                            CheckForUpdates(mainWindow);
                                        };
                                        mainWindow.textBlockUpdateStatus.MouseUp += textBlockUpdateStatus_MouseUp_Recheck;
                                        break;
                                    }
                            }
                        }));
                });
        }

        public static void SendConsoleMessage(this MainWindow mainWindow)
        {
            if (mainWindow.autoCompleteBoxConsoleCommand.Text.Length > 0)
            {
                if (mainWindow.autoCompleteBoxConsoleCommandSuggestions.IndexOf(mainWindow.autoCompleteBoxConsoleCommand.Text) < 0)
                {
                    mainWindow.autoCompleteBoxConsoleCommandSuggestions.Add(mainWindow.autoCompleteBoxConsoleCommand.Text);
                    ApplyDataBinding(mainWindow);
                }
                if (mainWindow.tabControlServersLogs.SelectedIndex > 0)
                {
                    IServer server = mainWindow._tabItems[(mainWindow.tabControlServersLogs.SelectedItem as TabItem)];
                    if (server.ServerProcess != null)
                    {
                        if (server.ServerProcess.StartInfo.FileName != "")
                        {
                            if (server.ServerProcess.Responding)
                            {
                                SendMessageUtil.SendTextMessage(mainWindow.autoCompleteBoxConsoleCommand.Text, server.ServerProcess);
                                Thread.Sleep(50);
                                ((mainWindow.tabControlServersLogs.SelectedItem as TabItem).Content as RichTextBox).ScrollToEnd();
                            }
                        }
                    }
                }
                else
                {
                    foreach (IServer server in mainWindow._serversControl.Servers)
                    {
                        if (server.ServerProcess != null)
                        {
                            if (server.ServerProcess.StartInfo.FileName != "")
                            {
                                if (server.ServerProcess.Responding)
                                {
                                    SendMessageUtil.SendTextMessage(mainWindow.autoCompleteBoxConsoleCommand.Text, server.ServerProcess);
                                }
                            }
                        }
                    }
                }
                mainWindow.autoCompleteBoxConsoleCommand.Focus();
            }
        }

        public static void ApplyDataBinding(this MainWindow mainWindow)
        {
            mainWindow.autoCompleteBoxConsoleCommand.ItemsSource = null;
            mainWindow.autoCompleteBoxConsoleCommand.ItemsSource = mainWindow.autoCompleteBoxConsoleCommandSuggestions;
        }
    }
}
