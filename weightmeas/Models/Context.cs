using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using weightmeas.Models;

namespace weightmeas.Models
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<WeightPlot> WeightPlots { get; set; }

        public Context() : base("context") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, MigrationConfiguration>());

            base.OnModelCreating(modelBuilder);
        }
    }
}