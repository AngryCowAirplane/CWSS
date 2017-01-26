using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cwssWpf.DataBase
{
    public static class Db
    {
        //public static Context dataBase;
        public static _BaseDataObject dataBase = _DataBase.Data;

        public static void Initialize()
        {
            LoadDatabase();
        }

        public static User GetUser(int loginId)
        {
            try
            {
                return dataBase.Users.Where(user => user.LoginId == loginId).First();
            }
            catch
            {
                return null;
            }
        }

        public static bool AddUser(string firstName, string LastName, int userId, string password1, string password2,
            string email, string address, string city, string state, int zip, string phone, GenderType gender, UserType userType = UserType.Patron)
        {
            try
            {
                var findUser = Db.GetUser(userId);
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

                    dataBase.Users.Add(user);
                    dataBase.SaveChanges();
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

        public static bool AddUser(string firstName, string LastName, string userId, string password1, string password2,
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

        public static bool AddUser(TextBox firstName, TextBox LastName, TextBox userId, PasswordBox password1, PasswordBox password2,
            TextBox email, TextBox address, TextBox city, TextBox state, TextBox zip, TextBox phone, ComboBox gender)
        {
            return AddUser(
                firstName.Text, LastName.Text,
                userId.Text, password1.Password, password2.Password,
                email.Text, address.Text, city.Text, state.Text, zip.Text,
                phone.Text, (GenderType)gender.SelectedItem
                );
        }

        public static bool AddUser(User user)
        {
            try
            {
                dataBase.Users.Add(user);
                dataBase.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void AddDefaultAdminUser(int DefaultAdminId, string DefaultAdminPassword)
        {
            AddUser("Admin", "User", DefaultAdminId, DefaultAdminPassword, DefaultAdminPassword, "admin@admin.com", "local admin", "noCity", "ZZ", 12345, "123-456-7890", GenderType.Female, UserType.Admin);
        }

        public static void LoadDatabase()
        {
            var dbPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", @"CwssDataBase.cwdb");
            _DataBase.Load(dbPath);
            dataBase = _DataBase.Data;
        }

        public static void LoadDatabase(string dbPath)
        {
            _DataBase.Load(dbPath);
            dataBase = _DataBase.Data;
        }

        public static void CreateNewDatabase(string dbPath)
        {
            _DataBase.CreateNew(dbPath);
            dataBase = _DataBase.Data;
        }

        public static _BaseDataObject GetNewDatabase(string dbPath)
        {
            CreateNewDatabase(dbPath);
            return dataBase;
        }

        public static bool DeleteUser(User user)
        {
            if (dataBase.Users.Contains(user))
            {
                Logger.Log(MainWindow.CurrentUser.LoginId, LogType.DeleteUser, user.UserType.ToString() + ", " + user.GetName() + " (" + user.LoginId + ") Was Deleted");
                dataBase.Users.Remove(user);
                return true;
            }
            else
                return false;
        }


        //----------------------------------------------------------------------------
        // 
        public static void ResetUsers()
        {
            var list = new List<User>();
            foreach (var user in dataBase.Users)
            {
                list.Add(user);
            }
            foreach (var user in list)
            {
                dataBase.Users.Remove(user);
            }
        }
    }
}
