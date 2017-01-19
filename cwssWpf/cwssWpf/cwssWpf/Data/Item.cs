using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf.Data
{
    // Item that can be checked out by a patron / user
    public class Item
    {
        public int ItemId;
        public string Name;
        public bool Available;
    }
}
