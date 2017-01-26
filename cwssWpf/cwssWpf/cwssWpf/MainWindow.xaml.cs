using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Migrations;
using cwssWpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    // add name="DefaultConnection" providerName="MySql.Data.MySqlClient" connectionString="Server=localhost;Database=cwss;Uid=admin;Pwd=admin" />

    public partial class MainWindow : Window
    {
        // TODO:
        // find appropriate place and functionality for default admin user/password if DB is lost
        // need the ability for the admin to change this immediately so this no longer is accepted from
        // regular DB querying.
        private int DefaultAdminId = 12345;
        private string DefaultAdminPassword = "abc123";
        public static User CurrentUser = null;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(Environment.CurrentDirectory, "AppData"));
            InitializeComponent();
            
            // Other Loading/Initializing done here between Status texts
            StatusText.Text = "Loading...";
            menuLogOut_Click(this, null); // Hide Menus

            //--use custom DB
            Db.Initialize();
            Logger.Initialize();

            //--use entity framework DB
            //Db.dataBase = new Context();
            //Db.dataBase.Database.Log = delegate (string message) { Console.Write(message); };

            if (Db.dataBase.Users.Count < 1)
                Db.AddDefaultAdminUser(DefaultAdminId, DefaultAdminPassword);

            FocusManager.SetFocusedElement(this, tbLoginId);
            StatusText.Text = "Ready";
            //MiddleText.Text = "User Name";
            UpdateClimberStats();
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
            login.ShowDialog();
            if (login.Success)
                menuEmployeeLogIn.IsEnabled = false;
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // validate input in tbUserId
            // if valid checkIn()
            checkIn();
            UpdateClimberStats();
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
                var hasWaiver = user.HasWaiver();
                var canClimb = user.CanClimb;

                if(hasWaiver && canClimb)
                {
                    if(!user.CheckedIn)
                        user.CheckIn();
                    else
                        user.CheckOut();
                }
                else
                {
                    // TODO:
                    // FIND BETTER WAY TO RESOLVE THIS SHIT
                    if(!hasWaiver)
                    {
                        var waiver = new Waiver();
                        var signedWaiver = waiver.ShowDialog();
                        if((bool)signedWaiver)
                        {
                            user.AddWaiver();
                            tryCheckinUser();
                        }
                        else
                            MessageBox.Show("Waiver Missing / Required!");
                    }
                    if(!canClimb)
                    {
                        MessageBox.Show("Climbing Priveleges Revoked");
                        // Show better reason, have Comment variable in User Table?
                    }
                }
            }
            else
            {
                var message = "Failed Checkin By " + loginId;
                Logger.Log(loginId, LogType.Error, message);
                MessageBox.Show("User Not Found!");
            }

            return true;
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
            menuEmployeeLogIn.IsEnabled = true;

            if(CurrentUser!=null)
            {
                var message = CurrentUser.GetName() + " Logged Off";
                Logger.Log(CurrentUser.UserId, LogType.LogOut, message);
            }

            CurrentUser = null;
        }

        private void menuMessageSystem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuCalendar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuListServ_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuManageUsers_Click(object sender, RoutedEventArgs e)
        {
            var userManager = new UserManager();
            userManager.Show();
        }

        private void menuAccounts_Click(object sender, RoutedEventArgs e)
        {
            var accountManager = new AccountManager();
            accountManager.Show();
        }

        private void menuStore_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void menuViewLog_Click(object sender, RoutedEventArgs e)
        {
            var logView = new LogView();
            logView.Show();
        }

        public void UpdateClimberStats()
        {
            StatsText.Text = "Climbers: " + Db.dataBase.Users.Where(t => t.CheckedIn == true).Count();
        }

        private void menuUsers_Click(object sender, RoutedEventArgs e)
        {
            var climberView = new ClimberView(this);
            climberView.Show();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentUser != null)
                menuLogOut_Click(null,null);
        }
    }
}
