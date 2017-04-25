using cwssWpf.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace cwssWpf.Network
{
    public static class Comms
    {
        public static MulticastUdpClient udpClientWrapper;
        public static event EventHandler <CustomCommArgs> CommPacketReceived;
        public static List<CommPacket> serverMessages = new List<CommPacket>();
        public static List<CommPacket> clientMessages = new List<CommPacket>();

        public static DispatcherTimer QuickTimer = new DispatcherTimer();

        public static int ClientPingCount = 0;
        public static int ServerPingCount = 0;

        private static CommPacket commPacket;

        public static void Initialize()
        {
            StartNetworkListen();
            QuickTimer.Interval = TimeSpan.FromMilliseconds(900);
            QuickTimer.Tick += OnQuickTimerTick;
            QuickTimer.Start();
        }

        private static void OnQuickTimerTick(object sender, EventArgs e)
        {
            if(serverMessages.Count > 0)
            {
                //notifyServer();
            }
            if(clientMessages.Count > 0)
            {
                //notifyClient();
            }
        }

        private static void StartNetworkListen()
        {
            // Create address objects
            int port = Int32.Parse(StaticValues.RemotePort);
            IPAddress multicastIPaddress = IPAddress.Parse(StaticValues.RemoteIP);
            IPAddress localIPaddress = IPAddress.Any;

            // Create MulticastUdpClient
            udpClientWrapper = new MulticastUdpClient(multicastIPaddress, port, localIPaddress);
            udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;
        }

        public static void SendMessage(CommPacket commPacket)
        {
            var message = Helpers.EncryptString(JsonConvert.SerializeObject(commPacket));
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            udpClientWrapper.SendMulticast(buffer);
        }

        public static void ResetConnection()
        {
            var message = "resetPingPackets";
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            udpClientWrapper.SendMulticast(buffer);
        }

        public static CommPacket GetMessage(Sender sender)
        {
            if (sender != commPacket.sender)
            {
                var packet = new CommPacket();
                packet = commPacket;
                commPacket = null;
                return packet;
            }
            else
                return null;
        }

        private static void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            try
            {
                string receivedText = ASCIIEncoding.Unicode.GetString(e.Buffer);
                var decryptedString = Helpers.DecryptString(receivedText);

                if(decryptedString == "resetPingPackets")
                {
                    ClientPingCount = 0;
                    ServerPingCount = 0;
                }

                commPacket = JsonConvert.DeserializeObject<CommPacket>(decryptedString);

                if (commPacket.sender == Sender.Server)
                    ClientPingCount = 0;
                if (commPacket.sender == Sender.Client)
                    ServerPingCount = 0;

                CommPacketReceived(null, new CustomCommArgs(commPacket.sender));
            }
            catch
            {

            }
        }

        public static dynamic GetObject(CommPacket packet)
        {
            if (packet.messageType == MessageType.CheckInResult)
                return JsonConvert.DeserializeObject<CheckinResult>(packet.messageObject);
            else if (packet.messageType == MessageType.Messages)
                return JsonConvert.DeserializeObject<MessagesPacket>(packet.messageObject);
            else if (packet.messageType == MessageType.NewUser)
                return JsonConvert.DeserializeObject<User>(packet.messageObject);
            else if (packet.messageType == MessageType.Waiver)
                return JsonConvert.DeserializeObject<WaiverPacket>(packet.messageObject);
            else if (packet.messageType == MessageType.CheckIn)
                return JsonConvert.DeserializeObject<string>(packet.messageObject);
            else if (packet.messageType == MessageType.ClientMode)
                return packet.messageObject;
            else
                return null;
        }
    }

    public class CommPacket
    {

        public Sender sender { get; set; }
        public MessageType messageType { get; set; }
        public string messageObject { get; set; }

        [JsonConstructor]
        public CommPacket()
        { }

        public CommPacket(Sender sender, CheckinResult result)
        {
            this.sender = sender;
            this.messageType = MessageType.CheckInResult;
            messageObject = JsonConvert.SerializeObject(result, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, MessagesPacket messages)
        {
            this.sender = sender;
            this.messageType = MessageType.Messages;
            messageObject = JsonConvert.SerializeObject(messages, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, WaiverPacket waiverPacket)
        {
            this.sender = sender;
            this.messageType = MessageType.Waiver;
            messageObject = JsonConvert.SerializeObject(waiverPacket, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, User user)
        {
            this.sender = sender;
            this.messageType = MessageType.NewUser;
            messageObject = JsonConvert.SerializeObject(user, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, string userId)
        {
            this.sender = sender;
            this.messageType = MessageType.CheckIn;
            messageObject = JsonConvert.SerializeObject(userId, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, bool clientClosed)
        {
            this.sender = sender;
            this.messageType = MessageType.ClientMode;
            messageObject = clientClosed.ToString();
        }

        public CommPacket(Sender sender)
        {
            this.sender = sender;
            this.messageType = MessageType.Ping;
            messageObject = null;
        }

        // Reset client commpacket
        public CommPacket(Sender sender, Sender client)
        {
            this.sender = sender;
            this.messageType = MessageType.Reset;
            messageObject = null;
        }
    }

    public enum Sender
    {
        Server = 0,
        Client = 1
    }

    public enum MessageType
    {
        CheckIn = 0,
        CheckInResult = 1,
        Messages = 2,
        Waiver = 3,
        NewUser = 4,
        ClientMode = 5,
        Ping = 6,
        Reset = 7
    }

    public class MessagesPacket
    {
        public List<Message> Messages;
        public User MessageUser;

        public MessagesPacket(List<Message> messages, User user)
        {
            this.Messages = messages;
            MessageUser = user;
        }
    }

    public class WaiverPacket
    {
        public User user;
    }

    public class CustomCommArgs : EventArgs
    {
        public Sender senderWindow;

        public CustomCommArgs(Sender sender)
        {
            senderWindow = sender;
        }
    }
}
