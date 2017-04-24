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
    /// Interaction logic for Email.xaml
    /// </summary>
    public partial class Email_Dialog : Window
    {
        private List<string> toList = new List<string>();

        public Email_Dialog(List<string> emailList)
        {
            toList = emailList;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            tbTo.Text = listToString(toList);
            tbTo.IsEnabled = false;
            tbFrom.Text = MainWindow.CurrentUser.GetEmailAddress();
            MouseLeftButtonDown += Helpers.Window_MouseDown;
            PreviewKeyDown += Helpers.HandleEsc;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            if (Config.Data.Email.StoreCreds)
            {
                Helpers.SendEmail(tbFrom.Text, toList, tbSubject.Text, tbBody.Text, Config.Data.Email.EmailAddress, Config.Data.Email.Password);
            }
            else
            {
                var emailCreds = new EmailCreds_Dialog();
                emailCreds.ShowDialog();
                Helpers.SendEmail(tbFrom.Text, toList, tbSubject.Text, tbBody.Text, emailCreds.LoginCred, emailCreds.Password);
            }

            this.Close();
            Helpers.PlaySuccess();
        }

        private string listToString(List<string> list)
        {
            string listString = string.Empty;
            foreach (var item in list)
            {
                listString += item + ", ";
            }
            return listString.Remove(listString.Length - 2, 1);
        }
    }
}
