using System.Data.Entity;
using System.Data.Entity.Validation;

using Timeline.Commands;
using Timeline.Events;
using Timeline.Snapshots;

namespace Sample.Persistence.Logs
{
    public class LogDbContext : DbContext
    {
        static LogDbContext()
        {
            Database.SetInitializer<LogDbContext>(null);
        }

        public LogDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<SerializedAggregate> Aggregates { get; set; }
        public DbSet<SerializedCommand> Commands { get; set; }
        public DbSet<SerializedEvent> Events { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new DbEntityException(e);
            }
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Configurations.Add(new AggregateConfiguration());
            builder.Configurations.Add(new CommandConfiguration());
            builder.Configurations.Add(new EventConfiguration());
            builder.Configurations.Add(new SnapshotConfiguration());
        }
    }
}
