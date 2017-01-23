using cwssWpf.Data;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cwssWpf
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Item> Items { get; set; }

        static Context()
        {
            Database.SetInitializer(new MySqlInitializer());
        }

        public Context() : base("DefaultConneciton")
        {

        }

        //// Constructor to use on a DbConnection that is already opened
        //public Context(DbConnection existingConnection, bool contextOwnsConnection)
        //    : base(existingConnection, contextOwnsConnection)
        //{

        //}
    }
}
