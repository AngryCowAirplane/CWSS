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

        public static void SaveDatabase(string dbPath)
        {
            _DataBase.Save(dbPath);
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
    }
}
