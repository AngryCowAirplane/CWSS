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
        public int RecipientId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
