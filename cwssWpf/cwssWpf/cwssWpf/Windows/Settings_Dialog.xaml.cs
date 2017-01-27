using cwssWpf.CustomControls;
using cwssWpf.Data;
using System;
using System.Collections.Generic;
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
        }

        private void addControls()
        {
            cbSsl = new ComboBool();
            cbSsl.Name = "cbSsl";
            cbSsl.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbSsl.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbSsl, 5);
            Grid.SetColumn(cbSsl, 1);

            cbDefaultCreds = new ComboBool();
            cbDefaultCreds.Name = "cbDefaultCreds";
            cbDefaultCreds.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbDefaultCreds.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbDefaultCreds, 6);
            Grid.SetColumn(cbDefaultCreds, 1);

            cbStoreCreds = new ComboBool();
            cbStoreCreds.Name = "cbStoreCreds";
            cbStoreCreds.HorizontalAlignment = HorizontalAlignment.Stretch;
            cbStoreCreds.Margin = new Thickness(5, 5, 5, 5);
            Grid.SetRow(cbStoreCreds, 7);
            Grid.SetColumn(cbStoreCreds, 1);

            SettingsGrid.Children.Add(cbSsl);
            SettingsGrid.Children.Add(cbDefaultCreds);
            SettingsGrid.Children.Add(cbStoreCreds);
        }

        private void saveConfig()
        {
            Config.Data.Email.Client = (EmailClient)cbClient.SelectedItem;
            Config.Data.Email.EnableSsl = cbSsl.Value;
            Config.Data.Email.SmtpPort = int.Parse(tbPort.Text);
            Config.Data.Email.SmtpServer = tbServer.Text;
            Config.Data.Email.UseDefaultCredentials = cbDefaultCreds.Value;
            Config.Data.Email.StoreCreds = cbStoreCreds.Value;

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
    }
}
