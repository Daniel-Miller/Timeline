using Microsoft.EntityFrameworkCore;
using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class QueryDbContext : DbContext
    {
        public QueryDbContext(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        public DbSet<AccountSummary> AccountSummaries { get; set; }
        
        public DbSet<PersonSummary> PersonSummaries { get; set; }
        
        public DbSet<TransferSummary> TransferSummaries { get; set; }

        public DbSet<UserSummary> UserSummaries { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AccountSummaryConfiguration());
            builder.ApplyConfiguration(new PersonSummaryConfiguration());
            builder.ApplyConfiguration(new TransferSummaryConfiguration());
            builder.ApplyConfiguration(new UserSummaryConfiguration());
        }
    }
}
