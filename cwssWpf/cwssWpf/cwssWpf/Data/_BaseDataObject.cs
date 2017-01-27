using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cwssWpf.Data
{
    // Essentially the main tables in the database
    // the individual classes(user, item, message, etc..) will be the lines (data members, columns..)

    public class _BaseDataObject
    {
        public List<User> Users = new List<User>();
        public List<Item> Items = new List<Item>();
        public List<Message> Messages = new List<Message>();
        public List<DailyLogTag> DailyLogs = new List<DailyLogTag>();

        public _BaseDataObject()
        {

        }

        public void AddDefaultAdminUser(int DefaultAdminId, string DefaultAdminPassword)
        {
            AddUser("Admin", "User", DefaultAdminId, DefaultAdminPassword, DefaultAdminPassword, "admin@admin.com", "local admin", "noCity", "ZZ", 12345, "123-456-7890", GenderType.Female, UserType.Admin);
        }

        public User GetUser(int loginId)
        {
            try
            {
                return Users.Where(user => user.LoginId == loginId).First();
            }
            catch
            {
                return null;
            }
        }

        public bool AddUser(string firstName, string LastName, int userId, string password1, string password2,
            string email, string address, string city, string state, int zip, string phone, GenderType gender, UserType userType = UserType.Patron)
        {
            try
            {
                var findUser = GetUser(userId);
                if (findUser == null)
                {
                    var user = new User();
                    user.Info.FirstName = firstName;
                    user.Info.LastName = LastName;
                    user.LoginId = userId;

                    if (password1 == password2)
                        user.Password = password1;
                    else
                        throw new Exception("Password Mismatch");

                    user.Info.Email = email;
                    user.Info.Address = address;
                    user.Info.City = city;
                    user.Info.State = state;
                    user.Info.Zip = zip.ToString();
                    user.Info.Phone = phone;
                    user.Info.Gender = gender;

                    user.CheckedIn = false;
                    user.CanClimb = true;
                    user.TimeStamp = DateTime.MinValue;
                    user.UserType = userType;

                    Users.Add(user);
                }
                else
                {
                    MessageBox.Show("User Already Exists.");
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddUser(string firstName, string LastName, string userId, string password1, string password2,
            string email, string address, string city, string state, string zip, string phone, GenderType gender)
        {
            try
            {
                var Id = int.Parse(userId);
                var Zip = int.Parse(zip);

                AddUser(firstName, LastName, Id, password1, password2, email, address, city, state, Zip, phone, gender);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool AddUser(TextBox firstName, TextBox LastName, TextBox userId, PasswordBox password1, PasswordBox password2,
            TextBox email, TextBox address, TextBox city, TextBox state, TextBox zip, TextBox phone, ComboBox gender)
        {
            return AddUser(
                firstName.Text, LastName.Text,
                userId.Text, password1.Password, password2.Password,
                email.Text, address.Text, city.Text, state.Text, zip.Text,
                phone.Text, (GenderType)gender.SelectedItem
                );
        }

        public bool AddUser(User user)
        {
            try
            {
                Users.Add(user);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteUser(User user)
        {
            if (Users.Contains(user))
            {
                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.DeleteUser, user.UserType.ToString() + ", " + user.GetName() + " (" + user.LoginId + ") Was Deleted");
                Users.Remove(user);
                return true;
            }
            else
                return false;
        }

        public bool AddItem()
        {
            return true;
        }

        public bool AddMessage(Message message)
        {
            Messages.Add(message);
            return true;
        }

        public List<Message> GetMessages(User user)
        {
            var messages = Messages.Where(message => message.RecipientId.Contains(user.LoginId)).ToList();
            return messages;
        }
    }
}
