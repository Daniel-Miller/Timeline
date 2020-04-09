using System.Data.Entity.ModelConfiguration;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class UserSummaryConfiguration : EntityTypeConfiguration<UserSummary>
    {
        public UserSummaryConfiguration() : this("queries") { }

        public UserSummaryConfiguration(string schema)
        {
            ToTable(schema + ".UserSummary");
            HasKey(x => new { x.UserIdentifier });

            Property(x => x.LoginName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LoginPassword).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserRegistrationStatus).IsRequired().IsUnicode(false).HasMaxLength(10);
        }
    }
}
