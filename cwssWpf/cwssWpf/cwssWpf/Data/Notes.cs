using cwssWpf.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class Notes
    {
        public List<Note> WallNotes = new List<Note>();
        public List<Request> Requests = new List<Request>();

        public List<Request> GetRequestsExpired()
        {
            var result = new List<Request>();
            foreach (var request in Requests)
            {
                if(calcTimeRemaining(request.TimeStamp, request.SuspensionLength) <= TimeSpan.Zero)
                {
                    result.Add(request);
                }
            }
            return result;
        }

        private TimeSpan calcTimeRemaining(DateTime start, Suspension length)
        {
            TimeSpan suspLength;
            TimeSpan timeElapsed;

            timeElapsed = DateTime.Now - start;

            switch(length)
            {
                case Suspension.Week:
                    suspLength = TimeSpan.FromDays(7);
                    break;
                case Suspension.Month:
                    suspLength = TimeSpan.FromDays(30);
                    break;
                case Suspension.Quarter:
                    suspLength = TimeSpan.FromDays(90);
                    break;
                case Suspension.Year:
                    suspLength = TimeSpan.FromDays(365);
                    break;
                default:
                    suspLength = TimeSpan.Zero;
                    break;
            }

            var timeLeft = suspLength - timeElapsed;
            return timeLeft;
        }

        public Request CheckRequest(User user)
        {
            var request = Requests.Where(u => u.Patron.LoginId == user.LoginId);
            if (request.Count() > 0)
                return request.First();
            else
                return null;
        }

        public void AddRequest(User user, string reason, Suspension length)
        {
            var request = new Request();
            request.Patron = user;
            request.Employee = MainWindow.CurrentUser;
            request.Reason = reason;
            request.SuspensionLength = length;
            request.TimeStamp = DateTime.Now;
            request.Enforced = false;

            if(Db.dataBase.Notes.Requests.Where(u => u.Patron == user).Count() <1)
                Db.dataBase.Notes.Requests.Add(request);
        }
    }

    public class Note
    {
        public string Contents;
        public double Top;
        public double Left;
        public double Width;
        public double Height;
        public int UserId;
    }

    public class Request
    {
        // NEED TO SWITCH THESE TO PATRON LOGIN and EMPLOYEE LOGIN
        public User Patron;
        public User Employee;
        public Suspension SuspensionLength;
        public string Reason;
        public DateTime TimeStamp;
        public bool Enforced = false;

        public void EnforceRequest()
        {
            Patron.CanClimb = false;
            Enforced = true;
        }

        public void ReleaseRequest()
        {
            var patron = Db.dataBase.Users.Where(user => user.LoginId == Patron.LoginId).First();
            patron.CanClimb = true;
            Db.dataBase.Notes.Requests.Remove(this);
        }
    }

    public enum Suspension
    {
        Week = 1,
        Month = 4,
        Quarter = 12,
        Year = 56
    }
}
