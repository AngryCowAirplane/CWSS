using cwssWpf.DataBase;
using cwssWpf.Windows;
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
        public List<Event> Events = new List<Event>();
        public Notes Notes = new Notes();

        public _BaseDataObject()
        {

        }

        public void AddDefaultAdminUser(int DefaultAdminId, string DefaultAdminPassword)
        {
            AddUser("Admin", "User", DefaultAdminId, DefaultAdminPassword, DefaultAdminPassword, "admin@admin.com", "local admin", "noCity", "ZZ", 12345, "123-456-7890", GenderType.Female, DateTime.MinValue ,"12345678901234567890", UserType.Admin);
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

        public User GetUser(string CardId)
        {
            try
            {
                return Users.Where(user => user.CardId == CardId).First();
            }
            catch
            {
                return null;
            }
        }

        public bool AddUser(string firstName, string LastName, int userId, string password1, string password2,
            string email, string address, string city, string state, int zip, string phone, GenderType gender, DateTime dob, string cardID, UserType userType = UserType.Patron)
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
                    user.Info.DateOfBirth = dob;
                    user.CardId = cardID;

                    user.CheckedIn = false;
                    user.CanClimb = true;
                    user.TimeStamp = DateTime.MinValue;
                    user.UserType = userType;

                    Users.Add(user);
                }
                else
                {
                    var alert = new Alert_Dialog("User Id Exists", "A user with the entered ID number already exists!");
                    alert.ShowDialog();
                    throw new Exception();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddUser(string firstName, string LastName, string userId, string password1, string password2,
            string email, string address, string city, string state, string zip, string phone, GenderType gender, DateTime dob, string cardID)
        {
            var success = false;
            try
            {
                var Id = int.Parse(userId);
                var Zip = int.Parse(zip);

                success = AddUser(firstName, LastName, Id, password1, password2, email, address, city, state, Zip, phone, gender, dob, cardID);
            }
            catch
            {
                return false;
            }
            return success;
        }

        public bool AddUser(TextBox firstName, TextBox LastName, TextBox userId, PasswordBox password1, PasswordBox password2,
            TextBox email, TextBox address, TextBox city, TextBox state, TextBox zip, TextBox phone, ComboBox gender, DatePicker dob, TextBox cardID)
        {
            return AddUser(
                firstName.Text, LastName.Text,
                userId.Text, password1.Password, password2.Password,
                email.Text, address.Text, city.Text, state.Text, zip.Text,
                phone.Text, (GenderType)gender.SelectedItem, (DateTime)dob.SelectedDate, cardID.Text
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

        public bool AddEvent(Event _event)
        {
            // check if conflicting event

            Events.Add(_event);
            return true;
        }

        public bool DeleteEvent(Event _event)
        {
            if (Events.Contains(_event))
            {
                Events.Remove(_event);
                return true;
            }
            else
                return false;
        }
    }
}
