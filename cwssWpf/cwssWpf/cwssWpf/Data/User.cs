using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class User
    {
        public int UserId;
        public string UserName;
        public string Password;
        public string Email;
        public string PhoneNumber;
        public UserType UserType;
        public bool CanClimb;
        public List<int> Items;

        public User()
        {
            Items = new List<int>();
        }

        public bool AddItem(Item item)
        {
            Items.Add(item.ItemId);
            return true;
        }

        public bool CheckWaiver()
        {
            return false;
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
