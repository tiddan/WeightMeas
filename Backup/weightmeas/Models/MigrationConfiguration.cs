using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace weightmeas.Models
{
    public class MigrationConfiguration : DbMigrationsConfiguration<weightmeas.Models.Context>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}