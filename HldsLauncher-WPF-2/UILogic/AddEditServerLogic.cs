using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;
using System.Net;
using HldsLauncher.Options;
using HldsLauncher.Enums;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using HldsLauncher.Utils;
using HldsLauncher.Servers;

namespace HldsLauncher.UILogic
{
    public static class AddEditServerLogic
    {
        public static void Browse(dynamic addEditServerWindow, string description, string executable)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = executable;
            dialog.Filter = string.Format("{0}|{1}", description, executable);
            if (dialog.ShowDialog() == true)
            {
                addEditServerWindow.serverOptions.ExecutablePath = dialog.FileName;
                if (executable != "hltv.exe")
                {
                    InitializeMapList(addEditServerWindow, dialog.FileName, true);
                }
            }
        }

        public static void InitializeFields(this AddEditServerSource addEditServerWindow)
        {
            addEditServerWindow.Dispatcher.Invoke(new Action(() =>
                {
                    addEditServerWindow.comboBoxServerGame.ItemsSource = typeof(SourceGame).ToList();
                    addEditServerWindow.comboBoxServerGame.SelectedIndex = 0;

                    addEditServerWindow.comboBoxConsoleType.ItemsSource = ConsoleTypes.ConsoleTypesList;
                    addEditServerWindow.comboBoxConsoleType.SelectedValue = ConsoleType.Integrated;

                    InitializeGeneralFields(addEditServerWindow);
                }));
        }

        public static void InitializeFields(this AddEditServerGoldSource addEditServerWindow)
        {
            addEditServerWindow.Dispatcher.Invoke(new Action(() =>
            {
                addEditServerWindow.comboBoxServerGame.ItemsSource = typeof(GoldSourceGame).ToList();
                addEditServerWindow.comboBoxServerGame.SelectedIndex = 0;

                addEditServerWindow.comboBoxConsoleType.ItemsSource = ConsoleTypes.ConsoleTypesList;
                addEditServerWindow.comboBoxConsoleType.SelectedValue = ConsoleType.Native;

                InitializeGeneralFields(addEditServerWindow);
            }));
        }

        public static void InitializeFields(this AddEditServerHltv addEditServerWindow)
        {
            addEditServerWindow.Dispatcher.Invoke(new Action(() =>
            {
                InitializeGeneralFields(addEditServerWindow);
            }));
        }

        private static void InitializeGeneralFields(dynamic addEditServerWindow)
        {
            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                addEditServerWindow.checkedListBoxProcessors.Items.Add(new ListBoxItem() { Content = string.Format(Properties.Resources.aes_Processor, i) });
            }

            addEditServerWindow.comboBoxServerIp.Items.Add("0.0.0.0");
            addEditServerWindow.comboBoxServerIp.Items.Add("127.0.0.1");
            foreach (IPAddress ip in Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    addEditServerWindow.comboBoxServerIp.Items.Add(ip);
                }
            }
            //addEditServerWindow.comboBoxServerIp.SelectedIndex = 0;

            addEditServerWindow.comboBoxPriority.ItemsSource = Priorities.PrioritiesList;
            addEditServerWindow.comboBoxPriority.SelectedItem = ProcessPriorityClass.Normal;

            FillProcessorsCheckedListBox(addEditServerWindow);
        }

        public static void InitializeMapList(dynamic addEditServerWindow, string executablePath, bool resetBusy)
        {
            if (executablePath != "")
            {
                List<string> maps = new List<string>();
                string selectedMap = addEditServerWindow.serverOptions.Map;
                string gameDir = addEditServerWindow.serverOptions.Game.ToString();
                addEditServerWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        addEditServerWindow.comboBoxServerMap.ItemsSource = null;
                    }));
                
                DirectoryInfo hlds = new DirectoryInfo(new FileInfo(executablePath).Directory.FullName + "\\"+ gameDir + "\\maps");
                if (hlds.Exists)
                {
                    FileInfo[] df1 = hlds.GetFiles("*.bsp");
                    foreach (FileInfo d in df1)
                    {
                        maps.Add(d.Name.Replace(".bsp", ""));
                    }
                }
                hlds = new DirectoryInfo(new FileInfo(executablePath).Directory.FullName + "\\" + gameDir + "_russian\\maps");
                if (hlds.Exists)
                {
                    FileInfo[] df2 = hlds.GetFiles("*.bsp");
                    foreach (FileInfo d in df2)
                    {
                        if (maps.IndexOf(d.Name.Replace(".bsp", "")) == -1)
                        {
                            maps.Add(d.Name.Replace(".bsp", ""));
                        }
                    }
                }
                addEditServerWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        addEditServerWindow.comboBoxServerMap.ItemsSource = maps;
                        if (addEditServerWindow.comboBoxServerMap.Items.Count > 0)
                        {
                            if (selectedMap != "" && addEditServerWindow.comboBoxServerMap.Items.IndexOf(selectedMap) >= 0)
                            {
                                addEditServerWindow.serverOptions.Map = selectedMap;
                            }
                            else
                            {
                                addEditServerWindow.comboBoxServerMap.SelectedIndex = 0;
                            }
                        }
                    }));
            }
        }

        public static void Save(this AddEditServerSource addEditServerWindow)
        {
            SaveGeneral(addEditServerWindow);
            addEditServerWindow.serverOptions.CloneObjectProps(addEditServerWindow.server.Options);
        }

        public static void Save(this AddEditServerGoldSource addEditServerWindow)
        {
            SaveGeneral(addEditServerWindow);
            addEditServerWindow.serverOptions.CloneObjectProps(addEditServerWindow.server.Options);
        }

        public static void Save(this AddEditServerHltv addEditServerWindow)
        {
            SaveGeneral(addEditServerWindow);
            addEditServerWindow.serverOptions.CloneObjectProps(addEditServerWindow.server.Options);
        }

        private static void SaveGeneral(dynamic addEditServerWindow)
        {
            SaveProcessorsCheckedListBox(addEditServerWindow);
        }

        private static void FillProcessorsCheckedListBox(dynamic addEditServerWindow)
        {
            foreach (ListBoxItem item in addEditServerWindow.checkedListBoxProcessors.Items)
            {
                item.IsSelected = false;
            }
            if (addEditServerWindow.serverOptions.ProcessorAffinity == IntPtr.Zero)
            {
                (addEditServerWindow.checkedListBoxProcessors.Items[0] as ListBoxItem).IsSelected = true;
            }
            int processorCount = Environment.ProcessorCount;
            char[] temp = new char[processorCount];
            char[] tempAffin = Convert.ToString((int)addEditServerWindow.serverOptions.ProcessorAffinity, 2).ToArray();
            tempAffin.CopyTo(temp, processorCount - tempAffin.Length);
            int k = processorCount - 1;
            for (int i = 0; i < processorCount; i++, k--)
            {
                if (temp[k] == '1')
                {
                    (addEditServerWindow.checkedListBoxProcessors.Items[i + 1] as ListBoxItem).IsSelected = true;
                }
            }
        }

        private static void SaveProcessorsCheckedListBox(dynamic addEditServerWindow)
        {
            if ((addEditServerWindow.checkedListBoxProcessors.Items[0] as ListBoxItem).IsSelected)
            {
                addEditServerWindow.serverOptions.ProcessorAffinity = IntPtr.Zero;
            }
            else
            {
                int processorCount = Environment.ProcessorCount;
                char[] temp = new char[processorCount];
                for (int i = 0; i < processorCount; i++)
                {
                    if ((addEditServerWindow.checkedListBoxProcessors.Items[i + 1] as ListBoxItem).IsSelected)
                        temp[i] = '1';
                    else
                        temp[i] = '0';
                }
                addEditServerWindow.serverOptions.ProcessorAffinity = BinTo16(temp);
            }
        }

        private static IntPtr BinTo16(char[] input)
        {
            String strResult = null;
            for (int i = 0; i < input.Length; i += 4)
            {
                char[] temp = { '0', '0', '0', '0' };
                int k = 3;
                for (int j = i; j < i + 4 && j < input.Length; j++, k--)
                {
                    temp[k] = input[j];
                }
                if (new String(temp) == "0000")
                {
                    strResult = 0.ToString() + strResult;
                }
                if (new String(temp) == "0001")
                {
                    strResult = 1.ToString() + strResult;
                }
                if (new String(temp) == "0010")
                {
                    strResult = 2.ToString() + strResult;
                }
                if (new String(temp) == "0011")
                {
                    strResult = 3.ToString() + strResult;
                }
                if (new String(temp) == "0100")
                {
                    strResult = 4.ToString() + strResult;
                }
                if (new String(temp) == "0101")
                {
                    strResult = 5.ToString() + strResult;
                }
                if (new String(temp) == "0110")
                {
                    strResult = 6.ToString() + strResult;
                }
                if (new String(temp) == "0111")
                {
                    strResult = 7.ToString() + strResult;
                }
                if (new String(temp) == "1000")
                {
                    strResult = 8.ToString() + strResult;
                }
                if (new String(temp) == "1001")
                {
                    strResult = 9.ToString() + strResult;
                }
                if (new String(temp) == "1010")
                {
                    strResult = "A" + strResult;
                }
                if (new String(temp) == "1011")
                {
                    strResult = "B" + strResult;
                }
                if (new String(temp) == "1100")
                {
                    strResult = "C" + strResult;
                }
                if (new String(temp) == "1101")
                {
                    strResult = "D" + strResult;
                }
                if (new String(temp) == "1110")
                {
                    strResult = "E" + strResult;
                }
                if (new String(temp) == "1111")
                {
                    strResult = "F" + strResult;
                }
            }
            return (IntPtr)Convert.ToInt32(strResult, 16);
        }

    }
}
