using cwssWpf.Data;
using cwssWpf.Network;
using cwssWpf.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace cwssWpf
{
    public partial class ClientWindow : Window
    {
        public static Dictionary<Window, TimerVal> WindowsOpen = new Dictionary<Window, TimerVal>();
        public static DispatcherTimer DasTimer = new DispatcherTimer();
        public static DispatcherTimer MidTimer = new DispatcherTimer();
        private bool serverConnected = false;

        public ClientWindow()
        {
            InitializeComponent();
            KeyUp += KeyPressed;

            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }

            StartNetworkListen(null, null);

            Comms.CommPacketReceived += Comms_CommPacketReceived;
            DasTimer.Interval = TimeSpan.FromMilliseconds(1666);
            DasTimer.Tick += OnTimerTick;
            DasTimer.Start();

            MidTimer.Interval = TimeSpan.FromMinutes(2);
            MidTimer.Tick += OnMidTimertick;
            MidTimer.Start();

            FocusManager.SetFocusedElement(this, tbLoginId);
            tbLoginId.Focus();
        }

        private void OnMidTimertick(object sender, EventArgs e)
        {
            if (!serverConnected)
                Comms.ResetConnection();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            FocusManager.SetFocusedElement(this, tbLoginId);
            tbLoginId.Focus();

            if (serverConnected)
            {
                tbLoginId.IsEnabled = true;
                btnCheckIn.IsEnabled = true;
                newUser.IsEnabled = true;
            }
            else
            {
                tbLoginId.IsEnabled = false;
                btnCheckIn.IsEnabled = false;
                newUser.IsEnabled = false;
            }

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
            if(serverConnected)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    var packet = new CommPacket(Sender.Client);
                    Comms.SendMessage(packet);
                    Comms.ClientPingCount++;
                }));
            }

            if (Comms.ClientPingCount > 5)
                serverConnected = false;
            else
                serverConnected = true;
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbLoginId.Text.Length > 0)
            {
                var packet = new CommPacket(Sender.Client, tbLoginId.Text);
                Comms.SendMessage(packet);
                tbLoginId.Text = string.Empty;
            }
        }

        private void newUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new NewUser_Dialog(this);
            newUser.ShowDialog();

            if (newUser.Success)
            {
                var user = newUser.NewUser;
                var packet = new CommPacket(Sender.Client, user);
                Comms.SendMessage(packet);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.X)
                {
                    Comms.CommPacketReceived -= Comms_CommPacketReceived;
                    this.Close();
                    var packet = new CommPacket(Sender.Client, true);
                    Comms.SendMessage(packet);
                    Application.Current.Shutdown();
                }
            }
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            btnCheckIn_Click(null, null);
        }

        private void StartNetworkListen(object sender, RoutedEventArgs e)
        {
            Comms.CommPacketReceived += Comms_CommPacketReceived;
        }

        private void Comms_CommPacketReceived(object sender, CustomCommArgs args)
        {
            if (args.senderWindow == Sender.Server)
            {
                var message = Comms.GetMessage(Sender.Client);
                if(message != null)
                {
                    var messageObject = Comms.GetObject(message);
                    if (message.messageType == MessageType.CheckInResult)
                    {
                        var result = (CheckinResult)messageObject;
                        if (result.Success)
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                var alert = new Alert_Dialog(result.Heading, result.Body, AlertType.Success);
                                WindowsOpen.Add(alert, new TimerVal(2));
                                alert.Show();
                            }));
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                var alert = new Alert_Dialog(result.Heading, result.Body, AlertType.Failure);
                                WindowsOpen.Add(alert, new TimerVal(3));
                                alert.Show();
                            }));
                        }
                    }
                    else if (message.messageType == MessageType.NewUser)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var newUser = new NewUser_Dialog(this);
                            newUser.ShowDialog();

                            if (true/*newUser.Success*/)
                            {
                                var user = newUser.NewUser;
                                var packet = new CommPacket(Sender.Client, user);
                                Comms.SendMessage(packet);
                            }
                        }));
                    }
                    else if (message.messageType == MessageType.Messages)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var messagePacket = (MessagesPacket)messageObject;
                            var user = messagePacket.MessageUser;
                            var messages = messagePacket.Messages;

                            var alert = new Alert_Dialog("Unread Messages!", "You have " + messages.Count + " messages.");
                            alert.ShowDialog();

                            foreach (var msg in messages)
                            {
                                var messageDialog = new Message_Dialog(user, msg);
                                messageDialog.ShowDialog();
                            }

                            var newMessagePacket = new MessagesPacket(messages, user);
                            var packet = new CommPacket(Sender.Client, newMessagePacket);
                            Comms.SendMessage(packet);

                            alert = new Alert_Dialog("Client Checkin", user.GetName() + " Checked In.", AlertType.Success);
                            WindowsOpen.Add(alert, new TimerVal(2));
                            alert.Show();
                        }));
                    }
                    else if (message.messageType == MessageType.Waiver)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var packet = (WaiverPacket)messageObject;
                            var user = packet.user;
                            var waiver = new Waiver_Dialog(user);
                            var signedWaiver = waiver.ShowDialog();
                            if ((bool)signedWaiver)
                            {
                                var waiverPac = new WaiverPacket();
                                waiverPac.user = user;
                                var newPacket = new CommPacket(Sender.Client, waiverPac);
                                Comms.SendMessage(newPacket);
                            }
                            else
                            {
                                var alert = new Alert_Dialog("Waiver Required", "Please see an employee to complete waiver agreement.", AlertType.Failure);
                                WindowsOpen.Add(alert, new TimerVal(6));
                                alert.Show();
                            }
                        }));
                    }
                    else if (message.messageType == MessageType.Ping)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Comms.ClientPingCount = 0;
                        }));
                    }
                    else if (message.messageType == MessageType.CheckIn)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if((string)messageObject == "failed")
                            {
                                var alert = new Alert_Dialog("New User Failed", "New user failed to create.  Please see employee.", AlertType.Failure);
                                WindowsOpen.Add(alert, new TimerVal(6));
                                alert.Show();
                            }
                            else
                            {
                                var alert = new Alert_Dialog("New User Created!", "Please sign waiver.", AlertType.Success);
                                WindowsOpen.Add(alert, new TimerVal(3));
                                alert.ShowDialog();
                                tbLoginId.Text = (string)messageObject;
                                btnCheckIn_Click(null, null);
                            }
                        }));
                    }

                    else if (message.messageType == MessageType.Reset)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Config.Data.General.StartClientMode = false;
                            Config.SaveConfigToFile();
                        }));
                    }
                }
            }
        }
    }
}