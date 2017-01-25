using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cwssWpf.Data
{
    public class User
    {
        public int UserId { get; set; }
        public int LoginId { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public PersonalInfo Info { get; set; }

        public bool CanClimb { get; set; }
        public bool CheckedIn { get; set; }
        public DateTime TimeStamp { get; set; }

        public List<int> Items;
        public List<Document> Documents;

        public User()
        {
            Info = new PersonalInfo();
            Items = new List<int>();
            Documents = new List<Document>();
        }

        public bool HasWaiver()
        {
            try
            {
                var waivers = Documents.Where(t => t.DocumentType == DocType.Waiver);
                var waiver = waivers.Where(t=>t.UserId == LoginId).First();
                if (waiver.Expires > DateTime.Now)
                    return true;
            }
            catch (Exception e)
            {
                // TODO:
                // throw exception up instead of MessageBox
                MessageBox.Show("Waiver Not Found!");
                return false;
            }
            return false;
        }

        public string GetName()
        {
            return Info.FirstName + " " + Info.LastName;
        }

    }

    public class PersonalInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public enum UserType
    {
        Patron = 0,
        Employee = 1,
        Manager = 2,
        Admin = 3
    }

    public enum GenderType
    {
        Male = 0,
        Female = 1
    }
}
