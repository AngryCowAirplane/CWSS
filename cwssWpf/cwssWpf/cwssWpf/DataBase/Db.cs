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

            CheckRequests();

            // Check Auto Backup
            if (DateTime.Now - Config.Data.Backup.LastBackup > TimeSpan.FromDays(Config.Data.Backup.DaysBetweenBackup))
            {
                var dbPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"AppData\Backup", @"CwssDataBase " + DateTime.Now.ToShortDateString().Replace('/', '_') + ".cwdb");
                Db.SaveDatabase(dbPath);
            }
        }

        public static void CheckRequests()
        {
            // Requests
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

        public static void CheckUserDocs(User user)
        {
            var docs = user.Documents;
            var lead = docs.Where(d => d.DocumentType == DocType.LeadClimb);//.First();
            var belay = docs.Where(d => d.DocumentType == DocType.BelayCert);//.First();
            var waiver = docs.Where(d => d.DocumentType == DocType.Waiver);//.First();

            //// Leads
            //if(lead != null)
            //{
            //    if (DateTime.Now > lead.Expires)
            //        user.Documents.Remove(lead);
            //}

            //// Belay
            //if(belay != null)
            //{
            //    if (DateTime.Now > belay.Expires)
            //        user.Documents.Remove(belay);
            //}

            // Wiaver
            if (waiver != null)
            {
                if (DateTime.Now > waiver.Last().Expires)
                    user.CanClimb = false;
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
