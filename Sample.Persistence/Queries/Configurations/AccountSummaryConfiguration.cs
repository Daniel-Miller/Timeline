using System.Data.Entity.ModelConfiguration;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class AccountSummaryConfiguration : EntityTypeConfiguration<AccountSummary>
    {
        public AccountSummaryConfiguration() : this("queries") { }

        public AccountSummaryConfiguration(string schema)
        {
            ToTable(schema + ".AccountSummary");
            HasKey(x => new { x.AccountIdentifier });

            Property(x => x.AccountCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AccountIdentifier).IsRequired();
            Property(x => x.AccountBalance).IsRequired();
            Property(x => x.OwnerIdentifier).IsRequired();
            Property(x => x.OwnerName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.TenantIdentifier).IsRequired();
        }
    }
}
