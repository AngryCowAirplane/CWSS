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

namespace cwssWpf
{
    public partial class ClientWindow : Window
    {
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
            FocusManager.SetFocusedElement(this, tbLoginId);
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if(tbLoginId.Text.Length > 0)
            {
                var packet = new CommPacket(Sender.Client, tbLoginId.Text);
                Comms.SendMessage(packet);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.X)
                {
                    //Comms.CommPacketReceived -= Comms_CommPacketReceived;
                    //this.Close();
                    //var packet = new CommPacket(Sender.Client, true);
                    //Comms.SendMessage(packet);
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
            //Comms.Initialize();
            Comms.CommPacketReceived += Comms_CommPacketReceived;
        }

        private void Comms_CommPacketReceived(object sender, CustomCommArgs args)
        {
            if (args.senderWindow == Sender.Server)
            {
                var message = Comms.GetMessage();
                var messageObject = Comms.GetObject(message);
                if (message.messageType == MessageType.CheckInResult)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                            messageObject.Show();
                    }));
                }
            }
        }

        //private void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        //{
        //    CheckinResult result = new CheckinResult();
        //    string receivedText = ASCIIEncoding.Unicode.GetString(e.Buffer);

        //    if (receivedText.Contains("Result^"))
        //    {
        //        var message = receivedText.Split(('^')).Last();
        //        result = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckinResult>(message);

        //        if (MainWindow.ClientMode)
        //        {
        //            Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //                result.Show();
        //            }));
        //        }
        //    }
        //    else if (receivedText.Contains("Message^"))
        //    {
        //        var parts = receivedText.Split(('^'));
        //        var userString = parts[1];
        //        var messagesString = parts.Last();

        //        var user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userString);
        //        var messages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Message>>(messagesString);

        //        if (MainWindow.ClientMode)
        //        {
        //            Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //                var alert = new Alert_Dialog("Unread Messages!", "You have " + messages.Count + " messages.");
        //                alert.ShowDialog();

        //                foreach (var msg in messages)
        //                {
        //                    var messageDialog = new Message_Dialog(user, msg);
        //                    messageDialog.ShowDialog();
        //                }

        //                var message = JsonConvert.SerializeObject(messages, Formatting.None, new JsonSerializerSettings()
        //                {
        //                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //                });

        //               // SendMessage("ReturnMessages^" + userString + "^" + message);
        //            }));
        //        }
        //    }
        //}
    }
}
