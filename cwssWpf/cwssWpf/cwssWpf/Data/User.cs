using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class User
    {
        public int UserId { get; set; }
        public int LoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public bool CanClimb { get; set; }
        public bool CheckedIn { get; set; }
        public DateTime TimeStamp { get; set; }
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
