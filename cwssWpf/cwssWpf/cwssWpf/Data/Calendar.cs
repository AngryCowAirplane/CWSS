using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace cwssWpf.Data
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventComment { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public List<User> EventMembers { get; set; }
        public User EventCreator { get; set; }

        public Event()
        {
            EventMembers = new List<User>();
        }

        public override string ToString()
        {
            return EventName;
        }
    }

    public class EventListViewItem : ListViewItem
    {
        public Event _Event { get; set; }
        public System.Windows.Media.Brush Color { get; set; }
        public EventType Type { get; set; }

    }

    public enum EventType
    {
        PrivateReservation = 0,
        ClimbingClinic = 1,
        Closed = 2,
        Other = 3
    }
}
