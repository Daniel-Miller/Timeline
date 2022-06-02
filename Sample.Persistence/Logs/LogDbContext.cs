using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Logs.Stores;
using Timeline.Events;
using Timeline.Snapshots;

namespace Sample.Persistence.Logs
{
    public class LogDbContext : DbContext
    {
        //static LogDbContext()
        //{
        //    Database.SetInitializer<LogDbContext>(null);
        //}

        public LogDbContext(DbContextOptions<LogDbContext> option)
            :base(option)
        {
        }

        public LogDbContext(string connectionString)
            : base(new DbContextOptionsBuilder<LogDbContext>().UseSqlServer(connectionString).Options)
        {           
        }

        public DbSet<SerializedAggregate> Aggregates { get; set; }
        public DbSet<SerializedCommand> Commands { get; set; }
        public DbSet<SerializedEvent> Events { get; set; }
        public DbSet<SerializedSnapShot> Snapshots { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AggregateConfiguration());
            builder.ApplyConfiguration(new CommandConfiguration());
            builder.ApplyConfiguration(new EventConfiguration());
            builder.ApplyConfiguration(new SnapshotConfiguration());
        }
    }
}
