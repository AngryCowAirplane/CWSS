using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Migrations;
using cwssWpf.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cwssWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // mySQL infos
    // add name="DefaultConnection" providerName="MySql.Data.MySqlClient" connectionString="Server=localhost;Database=cwss;Uid=root;Pwd=9087intxJON" />

    public partial class MainWindow : Window
    {
        // TODO:
        // find appropriate place and functionality for default admin user/password if DB is lost
        // need the ability for the admin to change this immediately so this no longer is accepted from
        // regular DB querying.
        public int DefaultAdminId = 12345;
        public string DefaultAdminPassword = "abc123";

        public MainWindow()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(Environment.CurrentDirectory, "AppData"));

            InitializeComponent();
            
            // Other Loading/Initializing done here between Status texts
            StatusText.Text = "Loading...";

            Db.Initialize();
            Logger.Initialize();

            //Db.dataBase = new Context();
            //Db.dataBase.Database.Log = delegate (string message) { Console.Write(message); };

            if(Db.dataBase.Users.Count < 1)
                Db.AddUser(DefaultAdminId, UserType.Admin, DefaultAdminPassword, true, "Admin", "admin@admin.com","");

            FocusManager.SetFocusedElement(this, tbLoginId);
            StatusText.Text = "Ready";
        }

        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new NewUser(this);
            newUser.Show();
            // TODO:
            // Show Appropriate message in status bar
        }

        private void menuEmployeeLogIn_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login(this);
            login.Show();
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // validate input in tbUserId
            // if valid checkIn()
            checkIn();
        }

        // TODO:
        // Add event like above to call checkIn() when the
        // "Enter" Key is pressed from within the userId textBox

        private void checkIn()
        {
            var success = tryCheckinUser();
            // TODO:
            // LOG APPROPRIATE MESSAGE (both for success and failure)
            // Show Appropriate message in status bar

            // TODO:
            // Make Function to Check to see if any messages for the user
            // And display to user any messages
        }

        private bool tryCheckinUser()
        {
            var loginId = int.Parse(tbLoginId.Text);
            var user = Db.GetUser(loginId);
            if(user != null)
            {
                if(user.CanClimb)
                {
                    if(!user.CheckedIn)
                    {
                        user.TimeStamp = DateTime.Now;
                        user.CheckedIn = true;
                        var message = user.UserName + " Checked In @" + user.TimeStamp.ToShortTimeString();
                        MessageBox.Show(message);
                        Logger.Log(user.UserId, LogType.CheckIn, message);
                    }
                    else
                    {
                        var length = DateTime.Now - user.TimeStamp;
                        user.TimeStamp = DateTime.Now;
                        user.CheckedIn = false;
                        var message = user.UserName + " Checked Out @" + user.TimeStamp.ToShortTimeString() + "\nDuration: " + length.TotalMinutes.ToString() + " minutes.";
                        MessageBox.Show(message);
                        Logger.Log(user.UserId, LogType.CheckOut, message);
                    }
                }
                else
                {
                    MessageBox.Show("Climbing Priveleges Revoked");
                }
            }
            else
                MessageBox.Show("User Not Found!");

            return true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Background = Brushes.CornflowerBlue;
            EmployeeMenu.Visibility = Visibility.Hidden;
            ManagerMenu.Visibility = Visibility.Hidden;
            AdminMenu.Visibility = Visibility.Hidden;
        }
    }
}
