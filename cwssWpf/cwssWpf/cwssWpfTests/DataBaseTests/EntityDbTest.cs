using cwssWpf.Migrations;
using cwssWpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cwssWpf.Data;
using cwssWpf.DataBase;

namespace cwssWpfTests.DataBaseTests
{
    [TestClass()]
    public class EntityDbTest
    {
        [TestMethod()]
        public void _EntityDbTest()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());
            //Db.dataBase = new Context();
            //foreach (var item in Db.dataBase.Users)
            //{
            //    Console.WriteLine(item.UserName);
            //}
        }
    }
}
