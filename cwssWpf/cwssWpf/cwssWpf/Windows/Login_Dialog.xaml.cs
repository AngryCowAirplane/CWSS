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
    public partial class Login_Dialog : Window
    {
        MainWindow mainWindow;
        public bool Success = false;

        public Login_Dialog(MainWindow sender)
        {
            mainWindow = sender;
            InitializeComponent();
            this.Left = mainWindow.Left + 50;
            this.Top = mainWindow.Top + 50;
            FocusManager.SetFocusedElement(this, tbUserId);
            MouseLeftButtonDown += Helpers.Window_MouseDown;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(Helpers.ValidateIdInput(tbUserId.Text))
            {
                var loginId = int.Parse(tbUserId.Text);
                var user = Db.dataBase.GetUser(loginId);
                if ((user != null) && ((int)user.UserType > 0) && (tbPassword.Password == user.Password))
                {
                    MainWindow.CurrentUser = user;
                    mainWindow.MainMenu.Background = Brushes.OrangeRed;
                    mainWindow.EmployeeMenu.Visibility = Visibility.Visible;
                    if ((int)user.UserType > 1)
                        mainWindow.ManagerMenu.Visibility = Visibility.Visible;
                    if ((int)user.UserType > 2)
                        mainWindow.AdminMenu.Visibility = Visibility.Visible;

                    Success = true;
                    var message = user.GetName() + " Logged In"; 
                    Logger.Log(user.LoginId, LogType.LogIn, message);
                }
                else
                {
                    var message = "Failed Login By " + loginId;
                    Logger.Log(loginId, LogType.Error, message);
                    var alert = new Alert_Dialog("Login Failed", "Employee ID or Password Incorrect!");
                    alert.ShowDialog();
                }
                this.Close();
            }
            else
            {
                var alert = new Alert_Dialog("Invalid ID", "The ID entered is not a valid integer ID within account range.");
                MainWindow.WindowsOpen.Add(alert, new TimerVal(6));
                alert.ShowDialog();
                tbUserId.Text = string.Empty;
            }
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            btnSubmit_Click(null, null);
        }
    }
}
