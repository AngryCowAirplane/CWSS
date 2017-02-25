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
    // mySQL infos
    // add name="DefaultConnection" providerName="MySql.Data.MySqlClient" connectionString="Server=localhost;Database=cwss;Uid=admin;Pwd=admin" />

    public partial class MainWindow : Window
    {
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
            Config.Initialize();
            Db.Initialize();
            Logger.Initialize();

            //--use entity framework DB
            //Db.dataBase = new Context();
            //Db.dataBase.Database.Log = delegate (string message) { Console.Write(message); };

            if (Db.dataBase.Users.Count < 1)
                Db.dataBase.AddDefaultAdminUser(DefaultAdminId, DefaultAdminPassword);

            FocusManager.SetFocusedElement(this, tbLoginId);
            StatusText.Text = "Ready";
            //MiddleText.Text = "User Name";
            UpdateClimberStats();

            //TESTING ACTIONS - REMOVE LATER
            //Db.dataBase.Notes.Requests = new List<Request>();
        }

        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new NewUser_Dialog(this);
            newUser.ShowDialog();
            // TODO:
            // Show Appropriate message in status bar
        }

        private void menuEmployeeLogIn_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login_Dialog(this);
            login.ShowDialog();
            if (login.Success)
            {
                // TODO abstract this + quickMessageRead Window (for each message)
                // same code in Checkin logic.
                var messages = Db.dataBase.GetMessages(CurrentUser);
                if (messages.Count > 0)
                {
                    var alert = new Alert_Dialog("Unread Messages!", "You have " + messages.Count + " messages.");
                    alert.ShowDialog();
                }

                menuEmployeeLogIn.IsEnabled = false;
            }
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
            var user = Db.dataBase.GetUser(loginId);
            if(user != null)
            {
                var messages = Db.dataBase.GetMessages(user);
                if (messages.Count > 0)
                {
                    var alert = new Alert_Dialog("Unread Messages!", "You have " + messages.Count + " messages.");
                    alert.ShowDialog();

                    foreach (var message in messages)
                    {
                        var messageDialog = new Message_Dialog(user, message);
                        messageDialog.ShowDialog();
                    }
                }

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
                        var alert = new Alert_Dialog("Missing Waiver!", "Please read and sign the electronic waiver.");
                        alert.ShowDialog();

                        var waiver = new Waiver_Dialog();
                        var signedWaiver = waiver.ShowDialog();
                        if((bool)signedWaiver)
                        {
                            user.AddWaiver();
                            tryCheckinUser();
                        }
                        else
                        {
                            var newalert = new Alert_Dialog("Not Signed", "Waiver not signed!");
                            newalert.ShowDialog();
                        }
                    }
                    if(!canClimb)
                    {
                        var alert = new Alert_Dialog("Climbing Priveleges Revoked", "Sorry, your climbing priveleges have been revoked.  Check with a staff member for more information.");
                        alert.ShowDialog();
                        // Show better reason, have Comment variable in User Table?
                    }
                }
            }
            else
            {
                var message = "Failed Checkin By " + loginId;
                Logger.Log(loginId, LogType.Error, message);

                var alert = new Alert_Dialog("User Not Found!", "Please try again, or create a new account.");
                alert.ShowDialog();
            }

            return true;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Background = Brushes.Goldenrod;
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
            var calendar = new Calendar_Dialog();
            calendar.Show();
        }

        private void menuReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuListServ_Click(object sender, RoutedEventArgs e)
        {
            var listServ = new ListServ_Dialog();
            listServ.ShowDialog();
        }

        private void menuAccounts_Click(object sender, RoutedEventArgs e)
        {
            var accountManager = new AccountManager_Dialog();
            accountManager.ShowDialog();
        }

        private void menuStore_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings_Dialog();
            settings.ShowDialog();
        }

        private void menuViewLog_Click(object sender, RoutedEventArgs e)
        {
            var logView = new LogView_Dialog();
            logView.ShowDialog();
        }

        private void menuUsers_Click(object sender, RoutedEventArgs e)
        {
            var climberView = new ClimberView_Dialog(this);
            climberView.ShowDialog();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentUser != null)
                menuLogOut_Click(null,null);
        }

        public void UpdateClimberStats()
        {
            StatsText.Text = "Climbers: " + Db.dataBase.Users.Where(t => t.CheckedIn == true).Count();
        }

        private void menuMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message_Dialog();
            message.ShowDialog();
        }

        private void menuNotes_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
