using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Reports
{
    public static class Report
    {
        public static void ListUsers(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(path, "Users.txt")))
            {
                foreach (var user in Db.dataBase.Users)
                {
                    file.WriteLine(user.UserName + " - " + user.UserType.ToString());
                }
            }
        }
    }
}
