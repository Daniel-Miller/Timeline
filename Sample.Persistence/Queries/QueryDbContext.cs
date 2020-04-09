using System.Data.Entity;
using System.Data.Entity.Validation;

using Sample.Application.Read;
using Sample.Persistence.Logs;

namespace Sample.Persistence.Queries
{
    public class QueryDbContext : DbContext
    {
        static QueryDbContext()
        {
            Database.SetInitializer<QueryDbContext>(null);
        }

        public QueryDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<AccountSummary> AccountSummaries { get; set; }
        public DbSet<PersonSummary> PersonSummaries { get; set; }
        public DbSet<TransferSummary> TransferSummaries { get; set; }
        public DbSet<UserSummary> UserSummaries { get; set; }

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

            builder.Configurations.Add(new AccountSummaryConfiguration());
            builder.Configurations.Add(new PersonSummaryConfiguration());
            builder.Configurations.Add(new TransferSummaryConfiguration());
            builder.Configurations.Add(new UserSummaryConfiguration());
        }
    }
}
