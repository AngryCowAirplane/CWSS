﻿using cwssWpf.Data;
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
        public static DispatcherTimer QuickTimer = new DispatcherTimer();
        public static DispatcherTimer MidTimer = new DispatcherTimer();
        public static DispatcherTimer LongTimer = new DispatcherTimer();

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
            Logger.Initialize();
            Config.Initialize();
            Db.Initialize();
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
            QuickTimer.Interval = TimeSpan.FromSeconds(1);
            QuickTimer.Tick += OnQuickTimerTick;
            QuickTimer.Start();
            MidTimer.Interval = TimeSpan.FromMinutes(2);
            MidTimer.Tick += OnMidTimertick;
            MidTimer.Start();
            LongTimer.Interval = TimeSpan.FromHours(1);
            LongTimer.Tick += OnLongTimertick;
            LongTimer.Start();

            // Other
            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }

            Db.CleanMessages();
            LoadConnectionImage();
            UpdateClimberStats();
            FocusManager.SetFocusedElement(this, tbLoginId);
            Logger.Log(000000, LogType.Other, "Application Started");
            StatusText.Text = "Ready";

            checkClientStart(args);
            CurrentUser = null;
        }

        private void OnMidTimertick(object sender, EventArgs e)
        {
            if (ClientConnected == false)
                Comms.ResetConnection();
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

                    if (result.Alert.Title.ToString().ToLower().Contains("waiver"))
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
            newUser.ShowDialog();

            if (newUser.Success)
            {
                var alert = new Alert_Dialog("User Created!", "", AlertType.Success);
                MainWindow.WindowsOpen.Add(alert, new TimerVal(3));
                alert.Show();
            }
            //else
            //{
            //    var alert = new Alert_Dialog("Add User Failed", "");
            //    MainWindow.WindowsOpen.Add(alert, new TimerVal(3));
            //    alert.Show();
            //}
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
                userMenu.IsEnabled = true;
                userMenu.Visibility = Visibility.Visible;
                if (Config.Data.Misc.ClimberView.Open)
                {
                    menuUsers_Click(null, null);
                }
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
            userMenu.IsEnabled = false;
            userMenu.Visibility = Visibility.Hidden;

            if (CurrentUser != null)
            {
                Helpers.PlayLogOff();
                var message = CurrentUser.GetName() + " Logged Off";
                Logger.Log(CurrentUser.LoginId, LogType.LogOut, message);
            }

            foreach (var wnd in WindowsOpen)
            {
                if (wnd.Value.time == -1)
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
            var windows = WindowsOpen.Keys;
            var climbingWindows = windows.Where(w => w.Name.ToLower().Contains("climber")).ToList();
            if(climbingWindows == null || climbingWindows.Count <= 0)
            {
                var climberView = new ClimberView_Dialog(this);
                if (Config.Data.Misc.ClimberView.Open)
                {
                    climberView.Left = Config.Data.Misc.ClimberView.Left;
                    climberView.Top = Config.Data.Misc.ClimberView.Top;
                    climberView.Height = Config.Data.Misc.ClimberView.Height;
                    climberView.Width = Config.Data.Misc.ClimberView.Width;
                }
                WindowsOpen.Add(climberView, new TimerVal(-1));
                climberView.Show();
            }
            else
            {
                climbingWindows.First().Focus();
            }
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
        private void remoteNewUser_Click(object sender, RoutedEventArgs e)
        {
            var packet = new CommPacket(Sender.Server, new User());
            Comms.SendMessage(packet);
        }
        #endregion

        #region Other Event Handlers
        private void OnLongTimertick(object sender, EventArgs e)
        {
            // Periodic Save Db
            Db.SaveDatabase(Db.DbPath);

            // Periodic Update Docs
            Db.CheckRequests();
        }

        private void OnQuickTimerTick(object sender, EventArgs e)
        {
            // Check windows timing out.
            var wndList = new List<Window>();
            foreach (var wnd in WindowsOpen)
            {
                if (wnd.Value.time > 0)
                {
                    wnd.Value.time--;
                }
                else if (wnd.Value.time == 0)
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

            QuickTimer.Stop();
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
                    if (e.Key == Key.W)
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
                Db.CheckUserDocs(user);
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
                    var message = "Failed Checkin By " + tbLoginId.Text;
                    if (!hasWaiver)
                    {
                        result.Alert = new Alert_Dialog("Missing Waiver!", "Please read and sign the electronic waiver.");
                        message = message + ", missing waiver.";
                    }
                    if (!canClimb)
                    {
                        Helpers.PlayFail();
                        result.Alert = new Alert_Dialog("Climbing Priveleges Revoked", "Sorry, your climbing priveleges have been revoked.  Check with a staff member for more information.");
                        message = message + ", priveleges revoked.";
                    }
                    Logger.Log(int.Parse(tbLoginId.Text), LogType.CheckIn, message);
                }
            }
            else
            {
                var message = "Failed Checkin By " + tbLoginId.Text + ", User does not exist";
                Logger.Log(tbLoginId.Text, LogType.Error, message);
                result.Alert = new Alert_Dialog("User Not Found!", "Please try again, or create a new account.");
            }

            if (user != null)
                user.LastCheckIn = DateTime.Now;

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
                        if(user.HasWaiver())
                        {
                            var messagesPacket = new MessagesPacket(messages, user);
                            var packet = new CommPacket(Sender.Server, messagesPacket);
                            Comms.SendMessage(packet);
                        }
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

                if (message != null && message.sender == Sender.Client)
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
                            if (Helpers.ValidateIdInput(tbLoginId.Text))
                            {
                                var user = getUserFromCheckInText();
                                var success = tryCheckinUser(user, true);
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
                            if (newUser != null)
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
                            foreach (var item in messagePacket.Messages)
                            {
                                msgs.Where(m => m.TimeStamp == item.TimeStamp).First().RecipientId = item.RecipientId;
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
            var startClientFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", "ClientSetup.cfg");
            var startClientInstallFile = File.Exists(startClientFilePath);

            foreach (var arg in args)
            {
                if (arg.ToLower().Contains("resetclient"))
                {
                    Config.Data.General.StartClientMode = false;
                    Config.SaveConfigToFile();
                }
            }

            if(startClientInstallFile)
            {
                Config.Data.General.StartClientMode = true;
                Config.Data.General.StartMaximized = true;
                Config.SaveConfigToFile();

                File.Delete(startClientFilePath);
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
            //var alert = new Alert_Dialog("Debug Message", "Server Ping Count: " + Comms.ServerPingCount.ToString() + "\n" + "Client Ping Count: " + Comms.ClientPingCount.ToString(), AlertType.Notice);
            //WindowsOpen.Add(alert, new TimerVal(3));
            //alert.Show();
            //var waiver = new Waiver_Dialog(CurrentUser);
            //waiver.ShowDialog();
        }
        #endregion

        private void resetConnection_Click(object sender, RoutedEventArgs e)
        {
            Comms.ResetConnection();
        }
    }
}
