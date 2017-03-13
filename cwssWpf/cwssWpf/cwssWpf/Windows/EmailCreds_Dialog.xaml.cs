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
    /// Interaction logic for EmailCreds.xaml
    /// </summary>
    public partial class EmailCreds_Dialog : Window
    {
        public string LoginCred;
        public string Password;
        public bool Save = false;

        public EmailCreds_Dialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbUserId.Text = MainWindow.CurrentUser.GetEmailAddress();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            LoginCred = tbUserId.Text;
            Password = tbPassword.Password;
            Save = (bool)cbSave.IsChecked;
            if(Save)
            {
                Config.Data.Email.StoreCreds = Save;
                Config.Data.Email.EmailAddress = LoginCred;
                Config.Data.Email.Password = Password;
                Config.SaveConfigToFile();
            }
            this.Close();
        }
    }
}
