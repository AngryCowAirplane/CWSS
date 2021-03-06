﻿using cwssWpf;
using cwssWpf.DataBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public static class Logger
    {
        private static DailyLog todaysLog;
        private static DailyLogTag todaysLogTag;

        public static void Initialize()
        {
            todaysLog = new DailyLog();
            todaysLogTag = new DailyLogTag();
            initializeLogDirectory();
            LoadLog();
        }

        private static void initializeLogDirectory()
        {
            var logBasePath = Path.Combine(Environment.CurrentDirectory, "AppData", "Logs");
            var yearPath = Path.Combine(logBasePath, DateTime.Now.Year.ToString());
            var monthPath = Path.Combine(yearPath, DateTime.Now.Month.ToString());
            var day = DateTime.Now.ToShortDateString().Replace('/', '_');
            var todaysLogPath = Path.Combine(monthPath, (day + ".cwlog"));

            if(!Directory.Exists(yearPath))
                Directory.CreateDirectory(yearPath);
            if (!Directory.Exists(monthPath))
                Directory.CreateDirectory(monthPath);

            todaysLogTag.SaveLocation = todaysLogPath;
        }

        public static void Log(int userId, LogType type, string comment = "")
        {
            // TODO:
            // check to see if same day as todaysLog, if not save log and start a new one

            var log = new Log();
            log.Action = type;
            log.UserId = userId.ToString();
            log.Comment = comment;
            log.TimeStamp = DateTime.Now;

            todaysLog.Logs.Add(log);
        }

        public static void Log(string userId, LogType type, string comment = "")
        {
            // TODO:
            // check to see if same day as todaysLog, if not save log and start a new one

            var log = new Log();
            log.Action = type;
            log.UserId = userId;
            log.Comment = comment;
            log.TimeStamp = DateTime.Now;

            todaysLog.Logs.Add(log);
        }

        public static DailyLog GetTodaysLog()
        {
            return todaysLog;
        }

        public static DailyLog GetLog(DateTime date)
        {
            var log = new DailyLog();

            var logBasePath = Path.Combine(Environment.CurrentDirectory, "AppData", "Logs");
            var yearPath = Path.Combine(logBasePath, date.Year.ToString());
            var monthPath = Path.Combine(yearPath, date.Month.ToString());
            var day = date.ToShortDateString().Replace('/', '_');
            var logPath = Path.Combine(monthPath, (day + ".cwlog"));

            if (File.Exists(logPath))
            {
                var logString = File.ReadAllText(logPath);
                log = JsonConvert.DeserializeObject<DailyLog>(Helpers.DecryptString(logString));
            }
            else
            {
                log = null;
            }

            return log;
        }

        private static bool LoadLog()
        {
            if (File.Exists(todaysLogTag.SaveLocation))
            {
                var logString = File.ReadAllText(todaysLogTag.SaveLocation);
                todaysLog = JsonConvert.DeserializeObject<DailyLog>(Helpers.DecryptString(logString));
            }
            else
            {
                todaysLog = new DailyLog();
                todaysLog.LogDate = DateTime.Now;
            }

            todaysLogTag.LogDate = todaysLog.LogDate;
            return true;
        }

        public static bool SaveLog()
        {
            todaysLog.LogDate = DateTime.Now.Date;

            var logString = JsonConvert.SerializeObject(todaysLog);
            File.WriteAllText(todaysLogTag.SaveLocation, Helpers.EncryptString(logString));

            if (!Db.dataBase.DailyLogs.Contains(todaysLogTag))
                Db.dataBase.DailyLogs.Add(todaysLogTag);

            return true;
        }
    }

    // Save a list of these in main data base in order to retrieve Daily log files
    public class DailyLogTag
    {
        public DateTime LogDate;
        public string SaveLocation;
    }

    // Save daily log to files on file system, organize in folders by year / month
    public class DailyLog
    {
        public int DailyLogId;
        public DateTime LogDate;
        public List<Log> Logs;

        public DailyLog()
        {
            Logs = new List<Log>();
            LogDate = DateTime.Now;
        }
    }

    // Log everyday actions of users
    public class Log
    {
        public string UserId;
        public DateTime TimeStamp;
        public LogType Action;
        public string Comment;

        public override string ToString()
        {
            var logData = (TimeStamp.ToShortDateString() + " " + TimeStamp.ToShortTimeString() + " (" + Action.ToString() + ") - " + Comment);
            //Console.WriteLine(logData);
            return logData;
        }

        public string ToCSV()
        {
            return (UserId.ToString() + "," + TimeStamp.ToString() + "," + Action.ToString() + "," + Comment + "\n");
        }

        public static string GetCsvHeader()
        {
            return ("UserId,TimeStamp,ActionType,Comment,\n");
        }
    }

    // Could expand to track employee actions
    public enum LogType
    {
        CheckIn = 0,
        LogIn = 2,
        LogOut = 3,
        AddUser = 4,
        DeleteUser = 5,
        EditUser = 6,
        Calendar = 7,
        Message = 8,
        Certification = 9,
        Waiver = 10,
        Privilege = 11,
        DataBase = 12,
        Settings = 13,
        Other = 14,
        Error = 15
    }
}
