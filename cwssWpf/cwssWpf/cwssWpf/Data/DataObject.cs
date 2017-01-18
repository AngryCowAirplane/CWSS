using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class DataObject
    {
        public List<User> Users = new List<User>();
        public List<Item> Items = new List<Item>();
        public List<Message> Messages = new List<Message>();
        public List<Log> Logs = new List<Log>();

        public DataObject()
        {

        }

        public bool AddUser()
        {
            return true;
        }

        public bool AddItem()
        {
            return true;
        }

        public bool AddMessage()
        {
            return true;
        }

        public bool AddLog()
        {
            return true;
        }
    }
}
