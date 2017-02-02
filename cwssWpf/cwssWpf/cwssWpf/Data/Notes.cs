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
    }

    public class Note
    {
        public string Subject;
        public string Contents;
    }

    public class Request
    {
        public User Patron;
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
            Patron.CanClimb = true;
            Db.dataBase.Notes.Requests.Remove(this);
        }
    }

    public enum Suspension
    {
        Week = 0,
        Month = 1,
        Quarter = 2,
        Year = 3
    }
}
