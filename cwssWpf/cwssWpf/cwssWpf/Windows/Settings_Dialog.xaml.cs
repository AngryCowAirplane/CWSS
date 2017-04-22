using cwssWpf.CustomControls;
using cwssWpf.Data;
using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cwssWpf.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings_Dialog : Window
    {
        private ComboBool cbSsl;
        private ComboBool cbDefaultCreds;
        private ComboBool cbStoreCreds;

        public Settings_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            cbClient.ItemsSource = (Enum.GetValues(typeof(EmailClient)).Cast<EmailClient>().ToList());
            cbClient.SelectionChanged += clientChanged;
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            addControls();
            setControls();
        }

        private void setControls()
        {
            cbClient.SelectedItem = Config.Data.Email.Client;
            tbServer.Text = Config.Data.Email.SmtpServer;
            tbPort.Text = Config.Data.Email.SmtpPort.ToString();
            cbSsl.SelectedItem = Config.Data.Email.EnableSsl;
            cbDefaultCreds.SelectedItem = Config.Data.Email.UseDefaultCredentials;
            cbStoreCreds.SelectedItem = Config.Data.Email.StoreCreds;
            cbMaximized.IsChecked = Config.Data.General.StartMaximized;
            cbIsClient.IsChecked = Config.Data.General.StartClientMode;
            tbMinPwdLength.Text = Config.Data.Data.MinPasswordLength.ToString();
            tbWaiverExpireDays.Text = Config.Data.Data.DaysWaiverExpires.ToString();
            tbBackupDays.Text = Config.Data.Backup.DaysBetweenBackup.ToString();
            cbGetSignature.IsChecked = Config.Data.General.GetSignature;
            lblLastBackup.Content = lblLastBackup.Content + Config.Data.Backup.LastBackup.ToShortDateString();
            tbSigDelay.Text = Config.Data.General.SignatureWaitDelay.ToString();
        }

        private void addControls()
        {
            cbSsl = new ComboBool();
            cbSsl.Name = "cbSsl";
            cbSsl.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbSsl.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbSsl, 6);
            Grid.SetColumn(cbSsl, 1);

            cbDefaultCreds = new ComboBool();
            cbDefaultCreds.Name = "cbDefaultCreds";
            cbDefaultCreds.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbDefaultCreds.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbDefaultCreds, 7);
            Grid.SetColumn(cbDefaultCreds, 1);

            cbStoreCreds = new ComboBool();
            cbStoreCreds.Name = "cbStoreCreds";
            cbStoreCreds.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbStoreCreds.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbStoreCreds, 8);
            Grid.SetColumn(cbStoreCreds, 1);

            Email.Children.Add(cbSsl);
            Email.Children.Add(cbDefaultCreds);
            Email.Children.Add(cbStoreCreds);
        }

        private void saveConfig()
        {
            Config.Data.Email.Client = (EmailClient)cbClient.SelectedItem;
            Config.Data.Email.EnableSsl = cbSsl.Value;
            Config.Data.Email.SmtpPort = int.Parse(tbPort.Text);
            Config.Data.Email.SmtpServer = tbServer.Text;
            Config.Data.Email.UseDefaultCredentials = cbDefaultCreds.Value;
            Config.Data.Email.StoreCreds = cbStoreCreds.Value;

            Config.Data.General.StartMaximized = (bool)cbMaximized.IsChecked;
            Config.Data.General.StartClientMode = (bool)cbIsClient.IsChecked;
            Config.Data.General.GetSignature = (bool)cbGetSignature.IsChecked;
            Config.Data.General.SignatureWaitDelay = int.Parse(tbSigDelay.Text);

            Config.Data.Backup.DaysBetweenBackup = int.Parse(tbBackupDays.Text);

            Config.Data.Data.MinPasswordLength = int.Parse(tbMinPwdLength.Text);
            Config.Data.Data.DaysWaiverExpires = int.Parse(tbWaiverExpireDays.Text);

            if (!Config.Data.Email.StoreCreds)
            {
                Config.Data.Email.EmailAddress = string.Empty;
                Config.Data.Email.Password = string.Empty;
            }
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            saveConfig();
            Config.SaveConfigToFile();
            this.Close();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clientChanged(object sender, RoutedEventArgs e)
        {
            if((EmailClient)cbClient.SelectedItem == EmailClient.LocalClient)
            {
                tbServer.IsEnabled = false;
                tbPort.IsEnabled = false;
                cbDefaultCreds.IsEnabled = false;
                cbSsl.IsEnabled = false;
                cbStoreCreds.IsEnabled = false;
            }
            else
            {
                tbServer.IsEnabled = true;
                tbPort.IsEnabled = true;
                cbDefaultCreds.IsEnabled = true;
                cbSsl.IsEnabled = true;
                cbStoreCreds.IsEnabled = true;
            }
        }

        private void btnBackupNow_Click(object sender, RoutedEventArgs e)
        {
            var dbPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"AppData\Backup", @"CwssDataBase " + DateTime.Now.ToShortDateString().Replace('/','_') + ".cwdb");
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();

            saveDialog.FileName = System.IO.Path.GetFileName(dbPath);
            saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(dbPath);
            saveDialog.DefaultExt = ".cwdb";
            saveDialog.Filter = "Climbing Wall DataBase (.cwdb)|*.cwdb";

            var result = saveDialog.ShowDialog();
            if (result == true)
            {
                string fileName = saveDialog.FileName;
                Db.SaveDatabase(fileName);

                Config.Data.Backup.LastBackup = DateTime.Now;
                Config.SaveConfigToFile();
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var dbPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"AppData\Backup", @"CwssDataBase " + DateTime.Now.ToShortDateString().Replace('/', '_') + ".cwdb");
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();

            openDialog.FileName = System.IO.Path.GetFileName(dbPath);
            openDialog.InitialDirectory = System.IO.Path.GetDirectoryName(dbPath);
            openDialog.DefaultExt = ".cwdb";
            openDialog.Filter = "Climbing Wall DataBase (.cwdb)|*.cwdb";

            var result = openDialog.ShowDialog();
            if (result == true)
            {
                var confirm = new Confirm_Dialog(this, "Replace/Restore DB?");
                confirm.ShowDialog();

                if (confirm.Confirmed)
                {
                    var path = openDialog.FileName;
                    Db.LoadDatabase(path);

                    var alert = new Alert_Dialog("DataBase Restored", "DataBase was restored with: " + System.IO.Path.GetFileName(path).ToString(), AlertType.Success);
                    MainWindow.WindowsOpen.Add(alert, new TimerVal(3));
                    alert.Show();
                }
            }
        }

        private void btnSaveLogs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
