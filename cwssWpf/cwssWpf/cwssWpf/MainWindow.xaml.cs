using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Network;
using cwssWpf.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace cwssWpf
{
    public partial class MainWindow : Window
    {
        public static User CurrentUser = null;
        public static Dictionary<Window, TimerVal> WindowsOpen = new Dictionary<Window, TimerVal>();
        public static bool ClientWindowOpen = false;
        public static bool ClientMode = false;
        public static bool ClientConnected = false;
        public static DispatcherTimer DasTimer = new DispatcherTimer(); 

        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(Environment.CurrentDirectory, "AppData"));
            InitializeComponent();

            // Other Loading/Initializing done here between Status texts
            //-------------------------------------------------------------
            StatusText.Text = "Loading...";
            menuLogOut_Click(this, null); // Hide Menus
            setCurrentUserApplication();

            // Static Class Initializations
            Config.Initialize();
            Db.Initialize();
            Logger.Initialize();
            Comms.Initialize();

            if ((Db.dataBase.Users.Where(u => u.UserType == UserType.Admin)).Count() < 1)
                Db.dataBase.AddDefaultAdminUser(StaticValues.DefaultAdminId, StaticValues.DefaultAdminPassword);

            if (Logger.GetTodaysLog().Logs.Count == 0)
            {
                foreach (var user in Db.dataBase.Users.Where(u => u.CheckedIn == true)) 
                {
                    user.CheckOut();
                }
            }

            // Event Subscriptions
            KeyUp += KeyPressed;
            Comms.CommPacketReceived += Comms_CommPacketReceived;
            DasTimer.Interval = TimeSpan.FromSeconds(1);
            DasTimer.Tick += OnTimerTick;
            DasTimer.Start();

            // Other
            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }

            LoadConnectionImage();
            UpdateClimberStats();
            FocusManager.SetFocusedElement(this, tbLoginId);
            Logger.Log(000000, LogType.Other, "Application Started");
            StatusText.Text = "Ready";

            checkClientStart(args);
            CurrentUser = null;
            //--------------------------------------------------------------


            //TESTING ACTIONS - REMOVE LATER
            //Db.dataBase.Notes.Requests = new List<Request>();
        }

        #region UI Click Event Handlers  (Click Events)
        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbLoginId.Text))
            {
                if (Helpers.ValidateIdInput(tbLoginId.Text))
                {
                    var user = getUserFromCheckInText();
                    var result = tryCheckinUser(user);
                    WindowsOpen.Add(result.Alert, new TimerVal(3));
                    result.Alert.ShowDialog();

                    if(result.Alert.Title.ToString().ToLower().Contains("waiver"))
                    {
                        var signed = Helpers.ShowWaiver(user);
                        if (signed)
                            result = tryCheckinUser(user);
                        else
                        {
                            Helpers.PlayFail();
                            result.Alert = new Alert_Dialog("Not Signed", "Waiver not signed!, User not checked in!");
                            WindowsOpen.Add(result.Alert, new TimerVal(6));
                            result.Alert.ShowDialog();
                        }
                    }
                }
                else
                {
                    var alert = new Alert_Dialog("Invalid ID", "The ID entered is not a valid integer ID within account range.");
                    WindowsOpen.Add(alert, new TimerVal(6));
                    alert.ShowDialog();
                    tbLoginId.Text = string.Empty;
                }
            }
        }

        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new NewUser_Dialog(this);
            var user = newUser.ShowDialog();
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
                Logger.Log(CurrentUser.LoginId, LogType.LogOut, message);
            }

            foreach (var wnd in WindowsOpen)
            {
                if(wnd.Value.time == -1)
                    wnd.Key.Close();
            }

            WindowsOpen.Clear();

            CurrentUser = null;
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
            Comms.CommPacketReceived -= Comms_CommPacketReceived;
            var clientWindow = new ClientWindow();
            clientWindow.Show();

            var packet = new CommPacket(Sender.Client, false);
            Comms.SendMessage(packet);

            ClientWindowOpen = true;

            this.Close();
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

        private void resetClientMode_Click(object sender, RoutedEventArgs e)
        {
            if (ClientConnected)
            {
                var packet = new CommPacket(Sender.Server, Sender.Client);
                Comms.SendMessage(packet);
            }
        }
        #endregion

        #region Other Event Handlers
        private void OnTimerTick(object sender, EventArgs e)
        {
            // Check windows timing out.
            var wndList = new List<Window>();
            foreach (var wnd in WindowsOpen)
            {
                if(wnd.Value.time > 0)
                {
                    wnd.Value.time--;
                }
                else if(wnd.Value.time == 0)
                {
                    wnd.Key.Close();
                    wndList.Add(wnd.Key);
                }
            }
            foreach (var wnd in wndList)
            {
                WindowsOpen.Remove(wnd);
            }

            // Check Client Connection
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var packet = new CommPacket(Sender.Server);
                Comms.SendMessage(packet);
                Comms.ServerPingCount++;
            }));

            var oldStatus = ClientConnected;

            if (Comms.ServerPingCount > 5)
                ClientConnected = false;
            else
                ClientConnected = true;

            if (oldStatus != ClientConnected)
            {
                if (ClientConnected)
                    Helpers.PlayLogin();
                else
                    Helpers.PlayLogOff();

                LoadConnectionImage();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentUser != null)
                menuLogOut_Click(null, null);

            DasTimer.Stop();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key.Enter))
            {
                btnCheckIn_Click(null, null);
            }

            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
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

        #region Custom Methods  //MOVE TO DIFFERENT_NEW CLASS?, KEEP ONLY EVENT HANDLERS HERE
        private CheckinResult tryCheckinUser(User user, bool remote = false)
        {
            var result = new CheckinResult();

            if (user != null)
            {
                checkMessages(user, remote);

                var hasWaiver = user.HasWaiver();
                var canClimb = user.CanClimb;

                if (hasWaiver && canClimb)
                {
                    user.CheckIn();
                    var message = user.Info.FirstName + " " + user.Info.LastName + " Checked In.";
                    result.Alert = new Alert_Dialog("Check In", message, AlertType.Success);
                    result.Success = true;
                }
                else
                {
                    if (!hasWaiver)
                    {
                        result.Alert = new Alert_Dialog("Missing Waiver!", "Please read and sign the electronic waiver.");
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
                var message = "Failed Checkin By " + tbLoginId.Text;
                Logger.Log(int.Parse(tbLoginId.Text), LogType.Error, message);
                result.Alert = new Alert_Dialog("User Not Found!", "Please try again, or create a new account.");
            }

            tbLoginId.Text = "";
            UpdateClimberStats();

            result.Initialize();
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
                        var messagesPacket = new MessagesPacket(messages, user);
                        var packet = new CommPacket(Sender.Server, messagesPacket);
                        Comms.SendMessage(packet);
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
                WindowsOpen.Add(postit, new TimerVal(-1));
            }
        }

        private void LoadConnectionImage()
        {
            Comms.ClientPingCount = 6;
            Comms.ServerPingCount = 6;
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\Disconnect.png");
            if (ClientConnected)
                path = System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\Connect.png");

            b.UriSource = new Uri(path);
            b.EndInit();

            ConnectionImage.Source = b;
        }

        private User getUserFromCheckInText(string text = "")
        {
            var user = Helpers.getUserFromCheckInText(tbLoginId.Text);
            return user;
        }

        private void Comms_CommPacketReceived(object sender, CustomCommArgs args)
        {
            if (args.senderWindow == Sender.Client)
            {
                var message = Comms.GetMessage(Sender.Server);

                if (message.sender == Sender.Client)
                {
                    var messageObject = Comms.GetObject(message);

                    if (message.messageType == MessageType.ClientMode)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if ((string)messageObject.ToLower() == "false")
                                ClientMode = true;
                            else
                                ClientMode = false;
                        }));
                    }

                    else if (message.messageType == MessageType.CheckIn)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            tbLoginId.Text = messageObject;
                            if(Helpers.ValidateIdInput(tbLoginId.Text))
                            {
                                var user = getUserFromCheckInText();
                                var success = tryCheckinUser(user);
                                tbLoginId.Text = "";

                                if (success.Body.ToLower().Contains("waiver"))
                                {
                                    var waiverPac = new WaiverPacket();
                                    waiverPac.user = user;
                                    var packet = new CommPacket(Sender.Server, waiverPac);
                                    Comms.SendMessage(packet);
                                }
                                else
                                {
                                    var packet = new CommPacket(Sender.Server, success);
                                    Comms.SendMessage(packet);
                                }

                                if (success.Success)
                                {
                                    var alert = new Alert_Dialog("Client Checkin", user.GetName() + " Checked In.", AlertType.Success);
                                    WindowsOpen.Add(alert, new TimerVal(2));
                                    alert.Show();
                                }
                                else
                                {
                                    var alert = new Alert_Dialog("Client Checkin", "Failed Check In.\n" + success.Body, AlertType.Failure);
                                    WindowsOpen.Add(alert, new TimerVal(2));
                                    alert.Show();
                                }
                            }
                            else
                            {
                                tbLoginId.Text = string.Empty;
                                var checkinResult = new CheckinResult();
                                checkinResult.Alert = new Alert_Dialog("Invalid ID", "The ID entered is not a valid integer ID within account range.");
                                checkinResult.Heading = "Invalid ID";
                                checkinResult.Body = "The ID entered is not a valid integer ID within account range.";
                                checkinResult.Success = false;
                                var packet = new CommPacket(Sender.Server, checkinResult);
                                Comms.SendMessage(packet);

                                var alert = new Alert_Dialog("Failed Checkin!", "A user just attempted to login un-successfuly.", AlertType.Notice);
                                WindowsOpen.Add(alert, new TimerVal(2));
                                alert.Show();
                            }
                        }));
                    }

                    else if (message.messageType == MessageType.NewUser)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var newUser = Helpers.TryAddUser((User)messageObject);
                            if(newUser != null)
                            {
                                var packet = new CommPacket(Sender.Server, newUser.GetUserId().ToString());
                                Comms.SendMessage(packet);
                            }
                            else
                            {
                                var packet = new CommPacket(Sender.Server, "failed");
                                Comms.SendMessage(packet);
                            }
                        }));
                    }

                    else if (message.messageType == MessageType.Messages)
                    {
                        var messagePacket = (MessagesPacket)messageObject;
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            List<Message> msgs = Db.dataBase.GetMessages(messagePacket.MessageUser).ToList();
                            foreach (var item in msgs)
                            {
                                if (item.RecipientId.Contains(messagePacket.MessageUser.LoginId))
                                    msgs.Where(msg => msg.TimeStamp == item.TimeStamp).First().ReadMessage(messagePacket.MessageUser);
                            }
                        }));
                    }

                    else if (message.messageType == MessageType.Waiver)
                    {
                        var waiverDoc = (WaiverPacket)messageObject;
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var user = Db.dataBase.Users.Where(u => u.LoginId == waiverDoc.user.LoginId).First();
                            user.AddWaiver();
                            var success = tryCheckinUser(user);
                            var packet = new CommPacket(Sender.Server, success);
                            Comms.SendMessage(packet);

                            if (success.Success)
                            {
                                var alert = new Alert_Dialog("Client Checkin", user.GetName() + " Checked In.", AlertType.Success);
                                WindowsOpen.Add(alert, new TimerVal(2));
                                alert.Show();
                            }
                        }));
                    }

                    else if (message.messageType == MessageType.Ping)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Comms.ServerPingCount = 0;
                        }));
                    }
                }
            }
        }

        private void checkClientStart(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.ToLower().Contains("resetclient"))
                {
                    Config.Data.General.StartClientMode = false;
                    Config.SaveConfigToFile();
                }
            }

            if (Config.Data.General.StartClientMode)
                menuClient_Click(null, null);
        }

        private void setCurrentUserApplication()
        {
            var user = new User();
            user.LoginId = 000000;
            user.Info.FirstName = "CWSS";
            user.Info.LastName = "APP";
            CurrentUser = user;
        }
        #endregion

        #region TESTING
        private void TestSomething(object sender, RoutedEventArgs e)
        {
            var alert = new Alert_Dialog("Debug Message", "Server Ping Count: " + Comms.ServerPingCount.ToString() + "\n" + "Client Ping Count: " + Comms.ClientPingCount.ToString(), AlertType.Notice);
            WindowsOpen.Add(alert, new TimerVal(3));
            alert.Show();
        }
        #endregion

        private void remoteNewUser_Click(object sender, RoutedEventArgs e)
        {
            var packet = new CommPacket(Sender.Server, new User());
            Comms.SendMessage(packet);
        }
    }
}
