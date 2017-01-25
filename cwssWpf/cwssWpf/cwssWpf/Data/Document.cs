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
        public Type DocumentType;
    }

    public class WaiverDoc : Document
    {
        public DateTime Date { get; set; }
        public DateTime Expires { get; set; }
        public string FileLocation { get; set; }
    }

    public class BelayCert : Document
    {
        // ??
    }

    public class TrainerCert : Document
    {
        // ??
    }
}
