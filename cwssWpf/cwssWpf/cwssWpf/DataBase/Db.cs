using cwssWpf.Data;
using System;
using System.Collections.Generic;
using System.IO;
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
                var message = "DataBase Auto Backup: " + Path.GetFileName(dbPath);
                Logger.Log(0, LogType.DataBase, message);
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
                var message = req.Patron.GetName() + " Climbing Priveleges Auto Restored (expired).";
                Logger.Log(req.Patron.LoginId, LogType.Privilege, message);
            }
        }

        public static void CheckUserDocs(User user)
        {
            var docs = user.Documents;
            var lead = docs.Where(d => d.DocumentType == DocType.LeadClimb).ToList();//.First();
            var belay = docs.Where(d => d.DocumentType == DocType.BelayCert).ToList();//.First();
            var waiver = docs.Where(d => d.DocumentType == DocType.Waiver).ToList();//.First();

            // Leads
            if (lead != null && lead.Count > 0)
            {
                if (DateTime.Now > lead.Last().Expires)
                {
                    user.IsLead = false;
                    var message =  user.GetName() + " is no longer Lead Climber (expired).";
                    Logger.Log(user.LoginId, LogType.Certification, message);
                }
            }

            // Belay
            if (belay != null && belay.Count > 0)
            {
                if (DateTime.Now > belay.Last().Expires)
                {
                    user.IsBelay = false;
                    var message = user.GetName() + " is no longer Belay Certified (expired).";
                    Logger.Log(user.LoginId, LogType.Certification, message);
                }
            }

            // Wiaver
            if (waiver != null && waiver.Count > 0)
            {
                if (DateTime.Now > waiver.Last().Expires)
                {
                    user.CanClimb = false;
                    var message = user.GetName() + " waiver expired.";
                    Logger.Log(user.LoginId, LogType.Waiver, message);
                }
            }

            if (DateTime.Now - user.Info.DateOfBirth > TimeSpan.FromDays(365 * 18))
                user.Info.Guardian = null;
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
