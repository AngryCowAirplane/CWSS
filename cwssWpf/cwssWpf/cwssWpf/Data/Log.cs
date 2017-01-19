using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public static class Logger
    {
        private static DailyLog todaysLog;

        public static void Initialize()
        {
            // TODO:
            // Check for current daily log in file system
            // if no log found, create new daily log
        }

        public static void Log(int userId, LogType type, string comment = "")
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

        public static bool SaveLog()
        {
            todaysLog.LogDate = DateTime.Now.Date;

            // TODO:
            // create new DailyLogTag
            // generate save location based on logdate (probably should be a seperate function)
            // save to file
            // add tag to Database

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
    }

    // Log everyday actions of users
    public class Log
    {
        public int UserId;
        public DateTime TimeStamp;
        public LogType Action;
        public string Comment;
    }

    // Could expand to track employee actions
    public enum LogType
    {
        CheckIn = 0,
        CheckOut = 1,
        LogIn = 2,
        LogOut = 3,
    }
}
