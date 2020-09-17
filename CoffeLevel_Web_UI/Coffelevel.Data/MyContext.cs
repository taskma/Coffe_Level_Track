using System.Data.Entity.ModelConfiguration.Conventions;
using CoffeLevel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeLevel.Data
{
    public class MyContext : DbContext
    {
        public MyContext()
            : base("CoffeLevel")
        {
            Console.WriteLine(Guid.NewGuid());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Project>()
            //    .HasMany(c => c.Reports).WithMany(i => i.Projects)
            //    .Map(t => t.MapLeftKey("ProjectId")
            //        .MapRightKey("ReportID")
            //        .ToTable("ProjectReports"));
        }

        public DbSet<CoffeeLevelTable> CoffeeLevelTable { get; set; }
        public DbSet<CoffeeInfoTable> CoffeeInfoTable { get; set; }
        public DbSet<CoffeeCalcLevelTable> CoffeeCalcLevelTable { get; set; }
        public DbSet<CoffeeEmailTable> CoffeeEmailTable { get; set; }
        public DbSet<ReferanceLevelTable> ReferanceLevelTable { get; set; }
        




    }
}
