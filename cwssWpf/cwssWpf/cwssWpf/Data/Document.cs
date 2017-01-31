using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    public class Document
    {
        public int DocumentId;
        public int UserId;
        public DocType DocumentType;
        public DateTime Date { get; set; }
        public DateTime Expires { get; set; }
        public string FileLocation { get; set; }
    }

    public enum DocType
    {
        Waiver = 0,
        BelayCert = 1,
        LeadClimb = 2
    }
}
