using cwssWpf.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cwssWpf.Network
{
    public static class Comms
    {
        public static MulticastUdpClient udpClientWrapper;
        public static event EventHandler CommPacketReceived;

        private static CommPacket commPacket;

        public static void Initialize()
        {
            StartNetworkListen();
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

        public static CommPacket GetMessage()
        {
            return commPacket;
        }

        private static void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            string receivedText = ASCIIEncoding.Unicode.GetString(e.Buffer);
            var decryptedString = Helpers.DecryptString(receivedText);
            commPacket = JsonConvert.DeserializeObject<CommPacket>(decryptedString);
            CommPacketReceived(null, null);
        }

        public static dynamic GetObject(CommPacket packet)
        {
            if (packet.messageType == MessageType.CheckInResult)
                return JsonConvert.DeserializeObject<CheckinResult>(packet.messageObject);
            else if (packet.messageType == MessageType.Messages)
                return JsonConvert.DeserializeObject<List<Message>>(packet.messageObject);
            else if (packet.messageType == MessageType.NewUser)
                return JsonConvert.DeserializeObject<User>(packet.messageObject);
            else if (packet.messageType == MessageType.Waiver)
                return JsonConvert.DeserializeObject<Document>(packet.messageObject);
            else if (packet.messageType == MessageType.CheckIn)
                return JsonConvert.DeserializeObject<string>(packet.messageObject);
            else if (packet.messageType == MessageType.ClientClosed)
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

        public CommPacket(Sender sender, List<Message> messages)
        {
            this.sender = sender;
            this.messageType = MessageType.Messages;
            messageObject = JsonConvert.SerializeObject(messages, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, Document document)
        {
            this.sender = sender;
            this.messageType = MessageType.Waiver;
            messageObject = JsonConvert.SerializeObject(document, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public CommPacket(Sender sender, User user)
        {
            this.sender = sender;
            this.messageType = MessageType.Waiver;
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
            this.messageType = MessageType.ClientClosed;
            messageObject = "ClientClosed";
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
        ClientClosed = 5
    }
}
