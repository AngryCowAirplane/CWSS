using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class User
    {
        private int userId;
        private string userName;
        private UserType userType;
        private List<Item> Items;

        public User()
        {

        }

        public bool AddItem()
        {
            var newItem = new Item();
            Items.Add(newItem);
            return true;
        }
    }

    public enum UserType
    {
        Patron = 0,
        Employee = 1,
        Manager = 2,
        Admin = 3
    }
}
