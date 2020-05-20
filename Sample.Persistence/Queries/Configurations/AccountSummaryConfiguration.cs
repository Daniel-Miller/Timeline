using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class AccountSummaryConfiguration : IEntityTypeConfiguration<AccountSummary>
    {
        private readonly string _schema;

        public AccountSummaryConfiguration() : this("queries") { }

        public AccountSummaryConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AccountSummary> builder)
        {
            builder.ToTable("AccountSummary", _schema);
            builder.HasKey(x => new { x.AccountIdentifier });

            builder.Property(x => x.AccountCode).IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.AccountIdentifier).IsRequired();
            builder.Property(x => x.AccountBalance).IsRequired();
            builder.Property(x => x.OwnerIdentifier).IsRequired();
            builder.Property(x => x.OwnerName).IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.TenantIdentifier).IsRequired();
        }
    }
}
