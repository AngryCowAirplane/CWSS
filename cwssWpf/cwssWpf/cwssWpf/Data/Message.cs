using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public List<int> RecipientId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Subject { get; set; }
        public string Contents { get; set; }

        public Message()
        {
            RecipientId = new List<int>();
            TimeStamp = DateTime.Now;
        }

        public void SetSender(User user)
        {
            SenderId = user.LoginId;
        }

        public void SetRecipient(User user)
        {
            RecipientId.Add(user.LoginId);
        }

        public void SetRecipients(List<User> users)
        {
            foreach (var user in users)
            {
                RecipientId.Add(user.LoginId);
            }
        }

        public string ReadMessage(User user)
        {
            RecipientId.Remove(user.LoginId);
            return Contents;
        }
    }

    public enum MessageMode
    {
        Single = 0,
        Multi = 1,
        Employees = 2,
        Managers = 3,
        Everyone = 4
    }
}
