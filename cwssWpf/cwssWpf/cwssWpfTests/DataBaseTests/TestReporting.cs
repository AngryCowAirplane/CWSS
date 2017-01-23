using cwssWpf.Data;
using cwssWpf.DataBase;
using cwssWpf.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpfTests.DataBaseTests
{
    [TestClass()]
    public class TestReporting
    {
        [TestMethod()]
        public void _TestReporting()
        {
            _DataBase.Load();
            Report.ListUsers();
        }
    }
}
