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
using HldsLauncher.Utils;
using HldsLauncher.Options;
using HldsLauncher.Enums;

namespace HldsLauncher
{
    /// <summary>
    /// Interaction logic for DialogOptions.xaml
    /// </summary>
    public partial class DialogOptions : Window
    {
        private HldslOptions _options;

        public DialogOptions(HldslOptions options)
        {
            InitializeComponent();
            comboBoxLanguage.ItemsSource = Languages.LanguagesList;
            comboBoxLanguage.DisplayMemberPath = "Name";

            _options = options;

            checkBoxAutoStartWithWindows.IsChecked = AutoStartUtil.IsAutoStartEnabled("HldsLauncher2", Application.ResourceAssembly.Location);
            checkBoxAutoStartMinimized.IsChecked = AutoStartUtil.IsAutoStartMinimized("HldsLauncher2", "/StartMinimized");
            checkBoxAutoStartServers.IsChecked = _options.StartServersAfterProgramStart;
            checkBoxMMTimerStartUpSet.IsChecked = _options.MMTimerStartUpSet;
            checkBoxShowServerInfoInTabs.IsChecked = _options.ShowServerInfoInTabs;
            checkBoxShowTabsOnlyIfServerStarted.IsChecked = _options.ShowTabsOnlyIfServerStarted;
            textBoxWaitBeforeStartServers.Text = _options.WaitBeforeServersStart.ToString();
            comboBoxLanguage.SelectedValue = Languages.LanguagesList.FirstOrDefault(l => l.Value == _options.Language);

            textBoxERStatus.Text = (ErrorReportingUtil.Status) ? Properties.Resources.do_ErrorReportingStatusEnabled : Properties.Resources.do_ErrorReportingStatusDisabled;

            comboBoxLanguage.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                {
                    MessageBox.Show(Properties.Resources.do_ChangeLanguageNeedRestart, AssemblyInfoUtil.AssemblyProduct);
                };
        }

        private void checkBoxAutoStartServers_Checked(object sender, RoutedEventArgs e)
        {
            textBoxWaitBeforeStartServers.IsEnabled = ((CheckBox)sender).IsChecked.Value;
        }

        private void textBoxWaitBeforeStartServers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationUtil.IsNumber(e.Text);
        }

        private void buttonEREnable_Click(object sender, RoutedEventArgs e)
        {
            ErrorReportingUtil.Status = true;
            textBoxERStatus.Text = (ErrorReportingUtil.Status) ? Properties.Resources.do_ErrorReportingStatusEnabled : Properties.Resources.do_ErrorReportingStatusDisabled;
        }

        private void buttonERDisable_Click(object sender, RoutedEventArgs e)
        {
            ErrorReportingUtil.Status = false;
            textBoxERStatus.Text = (ErrorReportingUtil.Status) ? Properties.Resources.do_ErrorReportingStatusEnabled : Properties.Resources.do_ErrorReportingStatusDisabled;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxAutoStartWithWindows.IsChecked.Value == true)
            {
                AutoStartUtil.SetAutoStart("HldsLauncher2", Application.ResourceAssembly.Location + (checkBoxAutoStartMinimized.IsChecked == true ? " /StartMinimized" : ""));
            }
            else if (checkBoxAutoStartWithWindows.IsChecked.Value == false)
            {
                AutoStartUtil.UnSetAutoStart("HldsLauncher2");
            }
            
            //_options.AutoStartWithWindows = checkBoxAutoStartWithWindows.IsChecked.Value;
            _options.StartServersAfterProgramStart = checkBoxAutoStartServers.IsChecked.Value;
            _options.MMTimerStartUpSet = checkBoxMMTimerStartUpSet.IsChecked.Value;
            _options.ShowServerInfoInTabs = checkBoxShowServerInfoInTabs.IsChecked.Value;
            _options.ShowTabsOnlyIfServerStarted = checkBoxShowTabsOnlyIfServerStarted.IsChecked.Value;
            _options.WaitBeforeServersStart = ((textBoxWaitBeforeStartServers.Text.Length > 0) ? int.Parse(textBoxWaitBeforeStartServers.Text) : 30);
            _options.Language = (comboBoxLanguage.SelectedValue as LanguageWrapper).Value;
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}
