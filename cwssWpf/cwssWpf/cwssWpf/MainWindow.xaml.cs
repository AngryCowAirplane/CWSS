using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Migrations;
using cwssWpf.Network;
using cwssWpf.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
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
        public static User CurrentUser = null;
        public static List<Window> WindowsOpen = new List<Window>();
        public static bool ClientMode = false;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(Environment.CurrentDirectory, "AppData"));
            InitializeComponent();

            // Other Loading/Initializing done here between Status texts
            //-------------------------------------------------------------
            StatusText.Text = "Loading...";
            menuLogOut_Click(this, null); // Hide Menus

            // Static Class Initializations
            Config.Initialize();
            Db.Initialize();
            Logger.Initialize();

            // DataBase (don't change unless migrating to a real database with entity framework)
            #region Database Setup
            //--use entity framework DB
            //Db.dataBase = new Context();
            //Db.dataBase.Database.Log = delegate (string message) { Console.Write(message); };

            if (Db.dataBase.Users.Count < 1)
                Db.dataBase.AddDefaultAdminUser(StaticValues.DefaultAdminId, StaticValues.DefaultAdminPassword);
            #endregion

            // Event Subscriptions
            KeyUp += KeyPressed;

            // Other
            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }

            StartNetworkListen(null, null);
            UpdateClimberStats();
            FocusManager.SetFocusedElement(this, tbLoginId);
            StatusText.Text = "Ready";
            //--------------------------------------------------------------


            //TESTING ACTIONS - REMOVE LATER
            //Db.dataBase.Notes.Requests = new List<Request>();
        }

        #region UI Click Event Handlers
        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new NewUser_Dialog(this);
            newUser.ShowDialog();
        }

        private void menuEmployeeLogIn_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login_Dialog(this);
            login.ShowDialog();
            if (login.Success)
            {
                Helpers.PlayLogin();
                checkMessages(CurrentUser);
                loadNotes();
                menuEmployeeLogIn.IsEnabled = false;
            }
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            // validate input in tbUserId

            if (!string.IsNullOrWhiteSpace(tbLoginId.Text))
            {
                var result = tryCheckinUser();
                result.Show();
            }
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

            if (CurrentUser != null)
            {
                Helpers.PlayLogOff();
                var message = CurrentUser.GetName() + " Logged Off";
                Logger.Log(CurrentUser.UserId, LogType.LogOut, message);
            }

            foreach (var wnd in WindowsOpen)
            {
                wnd.Close();
            }

            WindowsOpen.Clear();

            CurrentUser = null;
        }

        private void menuMessageSystem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuCalendar_Click(object sender, RoutedEventArgs e)
        {
            var calendar = new Calendar_Dialog();
            calendar.ShowDialog();
        }

        private void menuReports_Click(object sender, RoutedEventArgs e)
        {
            var reports = new Reports_Dialog();
            reports.ShowDialog();
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

        private void menuClient_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new ClientWindow();
            clientWindow.Show();
            ClientMode = true;
            this.Hide();
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
                menuLogOut_Click(null, null);
        }

        private void menuMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message_Dialog();
            message.ShowDialog();
        }

        private void menuNotes_Click(object sender, RoutedEventArgs e)
        {
            var notes = new Notes_Dialog();
            notes.ShowDialog();
        }
        #endregion

        #region Other Event Handlers
        private void TestSomething(object sender, RoutedEventArgs e)
        {
            if (CheckinCanvas.IsVisible)
                CheckinCanvas.Visibility = Visibility.Hidden;
            else
                CheckinCanvas.Visibility = Visibility.Visible;
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            var result = new Result();
            if (e.Key != Key.Enter || string.IsNullOrWhiteSpace(tbLoginId.Text)) return;

            result = tryCheckinUser();

            result.Show();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.L)
                {
                    if (CurrentUser == null)
                        menuEmployeeLogIn_Click(null, null);
                    else
                        menuLogOut_Click(null, null);
                }

                if (e.Key == Key.X)
                    menuExit_Click(null, null);
                if (e.Key == Key.N)
                    menuNewUser_Click(null, null);

                if (CurrentUser != null && (int)CurrentUser.UserType > 0)
                {
                    if (e.Key == Key.P)
                        menuNotes_Click(null, null);
                    if (e.Key == Key.F1)
                        menuClient_Click(null, null);
                    if (e.Key == Key.V)
                        menuUsers_Click(null, null);
                    if (e.Key == Key.C)
                        menuCalendar_Click(null, null);
                }
                if (CurrentUser != null && (int)CurrentUser.UserType > 1)
                {
                    if (e.Key == Key.A)
                        menuAccounts_Click(null, null);
                    if (e.Key == Key.R)
                        menuReports_Click(null, null);
                    if (e.Key == Key.U)
                        menuUsers_Click(null, null);
                }

                if (CurrentUser != null && (int)CurrentUser.UserType > 2)
                {
                    if (e.Key == Key.D)
                        menuViewLog_Click(null, null);
                    if (e.Key == Key.S)
                        menuSettings_Click(null, null);
                }
            }
        }
        #endregion

        #region Custom Methods
        private Result tryCheckinUser(string userId, bool remote = false)
        {
            tbLoginId.Text = userId;
            return tryCheckinUser(remote);
        }

        private Result tryCheckinUser(bool remote = false)
        {
            var result = new Result();
            var loginId = int.Parse(tbLoginId.Text);
            var user = Db.dataBase.GetUser(loginId);
            if (user != null)
            {
                checkMessages(user, remote);

                var hasWaiver = user.HasWaiver();
                var canClimb = user.CanClimb;

                if (hasWaiver && canClimb)
                {
                    user.CheckIn();
                    var message = user.Info.FirstName + " " + user.Info.LastName + " Checked In.";
                    result.Alert = new Alert_Dialog("Check In", message);
                }
                else
                {
                    if (!hasWaiver)
                    {
                        Helpers.PlayFail();
                        result.Alert = new Alert_Dialog("Missing Waiver!", "Please read and sign the electronic waiver.");

                        var waiver = new Waiver_Dialog();
                        var signedWaiver = waiver.ShowDialog();
                        if ((bool)signedWaiver)
                        {
                            user.AddWaiver();
                            tryCheckinUser();
                        }
                        else
                        {
                            Helpers.PlayFail();
                            result.Alert = new Alert_Dialog("Not Signed", "Waiver not signed!");
                        }
                    }
                    if (!canClimb)
                    {
                        Helpers.PlayFail();
                        result.Alert = new Alert_Dialog("Climbing Priveleges Revoked", "Sorry, your climbing priveleges have been revoked.  Check with a staff member for more information.");
                    }
                }
            }
            else
            {
                var message = "Failed Checkin By " + loginId;
                Logger.Log(loginId, LogType.Error, message);
                Helpers.PlayFail();
                result.Alert = new Alert_Dialog("User Not Found!", "Please try again, or create a new account.");
            }

            tbLoginId.Text = "";
            UpdateClimberStats();

            return result;
        }

        public void UpdateClimberStats()
        {
            var climbersCount = Db.dataBase.Users.Where(t => t.CheckedIn == true).Count();
            StatsText.Text = "Climbers: " + climbersCount;
            tbClimbers.Content = climbersCount;
        }

        private void checkMessages(User user, bool remote = false)
        {
            List<Message> messages = Db.dataBase.GetMessages(user).ToList();
            if (messages.Count > 0)
            {
                if (!ClientMode && !remote)
                {
                    var alert = new Alert_Dialog("Unread Messages!", "You have " + messages.Count + " messages.");
                    alert.ShowDialog();

                    foreach (var message in messages)
                    {
                        var messageDialog = new Message_Dialog(user, message);
                        messageDialog.ShowDialog();
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var message = JsonConvert.SerializeObject(messages, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        SendMessage("Message@" + message);
                    }));
                }
            }
        }

        private void loadNotes()
        {
            foreach (var note in Db.dataBase.Notes.WallNotes)
            {
                var postit = new Postit_Dialog(note);
                postit.Show();
                WindowsOpen.Add(postit);
            }
        }

        MulticastUdpClient udpClientWrapper;
        private void StartNetworkListen(object sender, RoutedEventArgs e)
        {
            // Create address objects
            int port = Int32.Parse(StaticValues.RemotePort);
            IPAddress multicastIPaddress = IPAddress.Parse(StaticValues.RemoteIP);
            IPAddress localIPaddress = IPAddress.Any;

            // Create MulticastUdpClient
            udpClientWrapper = new MulticastUdpClient(multicastIPaddress, port, localIPaddress);
            udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;
        }

        void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            string receivedText = ASCIIEncoding.Unicode.GetString(e.Buffer).ToLower();

            if (receivedText.Contains("clientclosed") && ClientMode)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Show();
                    ClientMode = false;
                }));
            }

            if (!ClientMode)
            {
                if (receivedText.Contains("checkin"))
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var id = receivedText.Split(',').Last();
                        var success = tryCheckinUser(id, true);
                        success.Initialize();
                        var message = JsonConvert.SerializeObject(success, Formatting.None, new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        SendMessage("Result@" + message);
                    }));
                }
            }
        }

        private void SendMessage(string message)
        {
            //string msgString = String.Format(message);
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            udpClientWrapper.SendMulticast(buffer);
        }
        #endregion

        private void menuTest_Click(object sender, RoutedEventArgs e)
        {
            var net = new Network_Dialog();
            net.Show();
        }
    }
}
