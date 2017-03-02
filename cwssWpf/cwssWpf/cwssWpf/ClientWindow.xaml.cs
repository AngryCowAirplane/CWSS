using cwssWpf.Data;
using cwssWpf.Network;
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
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        MulticastUdpClient udpClientWrapper;

        public ClientWindow()
        {
            InitializeComponent();
            KeyUp += KeyPressed;

            if (Config.Data.General.StartMaximized)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("Remote Checkin Clicked " + tbLoginId.Text);
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.X)
                {
                    this.Close();
                    SendMessage("ClientClosed");
                }
            }
        }

        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            btnCheckIn_Click(null, null);
        }

        private void SendMessage(string message)
        {
            string msgString = String.Format("message");// {1}", Helpers.GetLocalIPAddress());
            byte[] buffer = Encoding.Unicode.GetBytes(msgString);
            // Send
            udpClientWrapper.SendMulticast(buffer);
        }

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

        private void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
