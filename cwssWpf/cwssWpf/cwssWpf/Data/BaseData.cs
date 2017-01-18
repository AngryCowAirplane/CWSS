using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    // Essentially the main tables in the database
    // the individual classes(user, item, message, etc..) will be the lines (data members, columns..)

    public class BaseData
    {
        public List<User> Users = new List<User>();
        public List<Item> Items = new List<Item>();
        public List<Message> Messages = new List<Message>();
        public List<Log> Logs = new List<Log>();

        public BaseData()
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
