using cwssWpf.Data;
using cwssWpf.DataBase;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow mainWindow;
        public Login(MainWindow sender)
        {
            mainWindow = sender;
            InitializeComponent();
            this.Left = mainWindow.Left + 50;
            this.Top = mainWindow.Top + 50;
            FocusManager.SetFocusedElement(this, tbUserId);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var loginId = int.Parse(tbUserId.Text);
            var user = Db.GetUser(loginId);
            if ((user != null) && ((int)user.UserType > 0) && (tbPassword.Password == user.Password))
            {
                mainWindow.CurrentUser = user;
                mainWindow.MainMenu.Background = Brushes.Crimson;
                mainWindow.EmployeeMenu.Visibility = Visibility.Visible;
                if ((int)user.UserType > 1)
                    mainWindow.ManagerMenu.Visibility = Visibility.Visible;
                if ((int)user.UserType > 2)
                    mainWindow.AdminMenu.Visibility = Visibility.Visible;

                var message = user.GetName() + " logged in @" + DateTime.Now.ToShortTimeString(); 
                Logger.Log(user.UserId, LogType.LogIn, message);
            }
            else
            {
                var message = "Failed Login by " + loginId + " @" + DateTime.Now.ToShortTimeString();
                Logger.Log(loginId, LogType.Error, message);
                MessageBox.Show("Invalid Login");
            }
            this.Close();
        }
    }
}
