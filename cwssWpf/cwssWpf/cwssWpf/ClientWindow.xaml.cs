using cwssWpf.Network;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace cwssWpf
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
            KeyUp += KeyPressed;
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

        MulticastUdpClient udpClientWrapper;
        private void SendMessage(string message)
        {
            string msgString = String.Format("message {1}", Helpers.GetLocalIPAddress());
            byte[] buffer = Encoding.Unicode.GetBytes(msgString);
            // Send
            udpClientWrapper.SendMulticast(buffer);
        }
    }
}
