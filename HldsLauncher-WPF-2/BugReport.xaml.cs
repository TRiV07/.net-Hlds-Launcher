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
using System.ComponentModel;
using System.Net.Mail;
using System.Security;
using System.Net;
using HldsLauncher.Utils;
using System.IO;
using HldsLauncher.Log;

namespace HldsLauncher
{
    /// <summary>
    /// Interaction logic for BugReport.xaml
    /// </summary>
    public partial class BugReport : Window
    {
        private Exception _exception;
        private static ILogger _logger = HldslLogManager.GetLogManager().GetCurrentClassLogger();

        public int Result { get; set; }

        public BugReport()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(Exception e)
        {
            _exception = e;
            _logger.ErrorException(Properties.Resources.br_WindowMessage, e);
            return this.ShowDialog();
        }

        private void buttonSendReport_Click(object sender, RoutedEventArgs e)
        {
            var message = new StringBuilder();
            message.AppendFormat("Дата: {0:yyyy-MM-dd hh:mm}\r\n", DateTime.Now);
            message.AppendFormat(".net Hlds Launcher v.{0}\r\n\r\nOS: {1}\r\n\r\nException info: {2}", AssemblyInfoUtil.AssemblyVersion, Environment.OSVersion.VersionString, _exception.ToString());

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            String Pas = "hldslbugreport";
            SecureString secPas = new SecureString();
            foreach (char ch in Pas)
                secPas.AppendChar(ch);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("nethldsl@gmail.com", secPas);
            smtpClient.SendCompleted += (object sender1, AsyncCompletedEventArgs e1) =>
                {
                    busyIndicator.IsBusy = false;
                    if (e1.Error != null)
                    {
                        MessageBox.Show(Properties.Resources.br_mb_Fail, Properties.Resources.br_mb_FailTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.br_mb_Successful, Properties.Resources.br_mb_SuccessfulTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                };

            MailMessage mailMessage = new MailMessage(
                    "hldsl@and1gaming.org.ua",
                    "triv@and1gaming.org.ua",
                    ".net Hlds Launcher 2 Bug",
                    message.ToString());

            FileInfo fullLogFile = new FileInfo(string.Format(@"Logs\{0:yyyy-MM-dd}\Full.log", DateTime.Now));
            if (fullLogFile.Exists)
            {
                mailMessage.Attachments.Add(new Attachment(fullLogFile.FullName));
            }

            try
            {
                busyIndicator.IsBusy = true;
                smtpClient.SendAsync(mailMessage, "");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.br_mb_Fail, Properties.Resources.br_mb_FailTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                busyIndicator.IsBusy = false;
            }
        }

        public static void SendReport(Exception exception)
        {
            var message = new StringBuilder();
            message.AppendFormat("Дата: {0:yyyy-MM-dd hh:mm}\r\n", DateTime.Now);
            message.AppendFormat(".net Hlds Launcher v.{0}\r\n\r\nOS: {1}\r\n\r\nException info: {2}", AssemblyInfoUtil.AssemblyVersion, Environment.OSVersion.VersionString, exception.ToString());

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            String Pas = "hldslbugreport";
            SecureString secPas = new SecureString();
            foreach (char ch in Pas)
                secPas.AppendChar(ch);
            smtpClient.Credentials = new NetworkCredential("nethldsl@gmail.com", secPas);
            smtpClient.SendCompleted += (object sender1, AsyncCompletedEventArgs e1) =>
            {
                if (e1.Error != null)
                {
                    MessageBox.Show(Properties.Resources.br_mb_Fail, Properties.Resources.br_mb_FailTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.br_mb_Successful, Properties.Resources.br_mb_SuccessfulTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            };

            MailMessage mailMessage = new MailMessage(
                    "hldsl@and1gaming.org.ua",
                    "triv@and1gaming.org.ua",
                    ".net Hlds Launcher 2 Bug",
                    message.ToString());

            FileInfo fullLogFile = new FileInfo(string.Format(@"Logs\{0:yyyy-MM-dd}\Full.log", DateTime.Now));
            if (fullLogFile.Exists)
            {
                mailMessage.Attachments.Add(new Attachment(fullLogFile.FullName));
            }

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch
            {
                MessageBox.Show(Properties.Resources.br_mb_Fail, Properties.Resources.br_mb_FailTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            Result = 5;
            Close();
        }

        private void buttonContinue_Click(object sender, RoutedEventArgs e)
        {
            Result = 0;
            Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Result = 10;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = busyIndicator.IsBusy;
        }
    }
}
