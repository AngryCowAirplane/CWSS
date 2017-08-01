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
        public static string DbPath;

        public static void Initialize()
        {
            DbPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AppData", @"CwssDataBase.cwdb");
            LoadDatabase();

            CheckUserRequests();

            // Check Auto Backup
            if (DateTime.Now - Config.Data.Backup.LastBackup > TimeSpan.FromDays(Config.Data.Backup.DaysBetweenBackup))
            {
                var dbPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"AppData\Backup", @"CwssDataBase " + DateTime.Now.ToShortDateString().Replace('/', '_') + ".cwdb");
                Db.SaveDatabase(dbPath);
            }
        }

        public static void CheckUserRequests()
        {
            var reqToRelease = new List<Request>();
            foreach (var req in Db.dataBase.Notes.Requests)
            {
                var length = TimeSpan.FromDays((int)req.SuspensionLength * 7);
                if(req.TimeStamp + length < DateTime.Now)
                {
                    reqToRelease.Add(req);
                }
            }

            foreach (var req in reqToRelease)
            {
                req.ReleaseRequest();
            }
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
