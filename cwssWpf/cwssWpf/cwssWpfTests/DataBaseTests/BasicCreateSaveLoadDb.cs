using Microsoft.VisualStudio.TestTools.UnitTesting;
using cwssWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cwssWpf.Data;
using System.IO;

namespace cwssWpf.Tests
{
    [TestClass()]
    public class BasicCreateSaveLoadDb
    {
        [TestMethod]
        public void _BasicCreateSaveLoadDb()
        {
            DataBase.Data = new BaseDataObject();

            var user = new User();
            user.UserId = 1023542;
            user.UserName = "Derek";
            user.UserType = UserType.Admin;
            DataBase.Data.AddUser(user);
            Assert.IsTrue(DataBase.Data.Users.Count > 0);

            DataBase.Save();
            Assert.IsTrue(File.Exists(DataBase.SavePath + DataBase.DbFileName));

            DataBase.Data = new Data.BaseDataObject();
            Assert.IsTrue(DataBase.Data.Users.Count == 0);

            DataBase.Load();
            Assert.IsTrue(DataBase.Data.Users.Count > 0);
        }
    }
}