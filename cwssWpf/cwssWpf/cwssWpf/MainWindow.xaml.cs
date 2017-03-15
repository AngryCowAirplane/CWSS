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
            Comms.Initialize();

            if (Db.dataBase.Users.Count < 1)
                Db.dataBase.AddDefaultAdminUser(StaticValues.DefaultAdminId, StaticValues.DefaultAdminPassword);

            // Event Subscriptions
            KeyUp += KeyPressed;
            Comms.CommPacketReceived += Comms_CommPacketReceived;

            // Other
            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }

            UpdateClimberStats();
            FocusManager.SetFocusedElement(this, tbLoginId);
            StatusText.Text = "Ready";
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
                    var result = tryCheckinUser();
                    result.Show();
                }
                else
                {
                    var alert = new Alert_Dialog("Invalid ID", "The ID entered is not a valid integer ID within account range.");
                    alert.ShowDialog();
                    tbLoginId.Text = string.Empty;
                }
            }
        }

        private void menuNewUser_Click(object sender, RoutedEventArgs e)
        {
            //if(ClientMode)  // can send new user form to client screen
            if(false)
            {
                var packet = new CommPacket(Sender.Server, new User());
                Comms.SendMessage(packet);
            }
            else
            {
                var newUser = new NewUser_Dialog(this);
                newUser.ShowDialog();
            }
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
                Logger.Log(CurrentUser.UserId, LogType.LogOut, message);
            }

            foreach (var wnd in WindowsOpen)
            {
                wnd.Close();
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
        #endregion

        #region Other Event Handlers
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentUser != null)
                menuLogOut_Click(null, null);
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
        private CheckinResult tryCheckinUser(string userId, bool remote = false)
        {
            tbLoginId.Text = userId;
            return tryCheckinUser(remote);
        }

        private CheckinResult tryCheckinUser(bool remote = false)
        {
            var result = new CheckinResult();
            if (tbLoginId.Text[0] == StaticValues.CardReaderStartChar)
                tbLoginId.Text = Helpers.TryGetCardId(tbLoginId.Text);

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
                    result.Alert = new Alert_Dialog("Check In", message, autoClose: true);
                    result.Success = true;
                    Helpers.PlayLogin();
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
                            if (remote)
                            {
                                // Start Remote Actions class to move everything remote related out of logic in this class.
                                // or seperate remote try checkin user function.
                            }
                            else
                            {
                                user.AddWaiver();
                                tryCheckinUser();
                            }
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
                WindowsOpen.Add(postit);
            }
        }

        private void Comms_CommPacketReceived(object sender, CustomCommArgs args)
        {
            if (args.senderWindow == Sender.Client)
            {
                var message = Comms.GetMessage();

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
                            var success = tryCheckinUser();
                            tbLoginId.Text = "";
                            var packet = new CommPacket(Sender.Server, success);
                            Comms.SendMessage(packet);
                            //success.ShowAuto();


                        }));
                    }

                    else if (message.messageType == MessageType.NewUser)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Db.dataBase.AddUser((User)messageObject);
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
                }
            }
        }
        #endregion

        #region TESTING
        private void TestSomething(object sender, RoutedEventArgs e)
        {
            if (CheckinCanvas.IsVisible)
                CheckinCanvas.Visibility = Visibility.Hidden;
            else
                CheckinCanvas.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
