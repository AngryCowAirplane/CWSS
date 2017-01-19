using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class Message
    {
        public int MessageId;
        public int SenderId;
        public int RecipientId = -1;        // -1 To All?
        public DateTime TimeStamp;
        public DateTime ExpireDate;
    }
}
