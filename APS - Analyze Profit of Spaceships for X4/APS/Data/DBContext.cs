using BusinessLogic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APS.Model
{
    public partial class DBContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<TradeOperation> TradeOperations { get; set; }
        
        public virtual DbSet<Ware> Wares { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }

        public static readonly Microsoft.Extensions.Logging.LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[] {
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = Methods.GlobalMethods.GetConfiguration();
                optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    mySqlOptionsAction => mySqlOptionsAction.CommandTimeout(5000));
                optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customizations must go after base.OnModelCreating(builder)
            
        }

    }
    
}
    