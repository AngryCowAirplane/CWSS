using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.DataBase
{
    public static class Db
    {
        public static Context dataBase;

        public static User GetUser(int loginId)
        {
            return dataBase.Users.Where(user => user.LoginId == loginId).First();
        }

        public static bool AddUser(int loginId, UserType userType, string password, bool canClimb = true, string userName="", string email="", string phone="")
        {
            var user = new User();
            user.CanClimb = canClimb;
            user.Email = email;
            user.Password = password;
            user.PhoneNumber = phone;
            user.UserName = userName;
            user.LoginId = loginId;
            user.UserType = userType;

            dataBase.Users.Add(user);
            dataBase.SaveChanges();
            return true;
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
