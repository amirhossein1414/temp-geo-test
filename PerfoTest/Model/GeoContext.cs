using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Model
{
    public class GeoContext : DbContext
    {
        public readonly static string ConnectionString =
            "data source=192.168.30.5;initial catalog = geo;  persist security info=True;user id = sa; password=!@#123qwe;MultipleActiveResultSets=True;App=EntityFramework";
        //"data source=localhost;initial catalog=geo; Trusted_Connection=True;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        public GeoContext() : base(ConnectionString)
        {
            //Database.SetInitializer<GeoContext>(new CreateDatabaseIfNotExists<GeoContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GeoContext, PerfoTest.Migrations.Configuration>());
        }

        public DbSet<SuperMarket> SuperMarkets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
