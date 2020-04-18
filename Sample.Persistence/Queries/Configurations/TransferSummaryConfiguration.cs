using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class TransferSummaryConfiguration : IEntityTypeConfiguration<TransferSummary>
    {
        private readonly string _schema;

        public TransferSummaryConfiguration() : this("queries") { }

        public TransferSummaryConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<TransferSummary> builder)
        {
            builder.ToTable("TransferSummary", _schema);
            builder.HasKey(x => new { x.TransferIdentifier });

            builder.Property(x => x.FromAccountIdentifier).IsRequired();
            builder.Property(x => x.FromAccountOwner).IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.TenantIdentifier).IsRequired();
            builder.Property(x => x.ToAccountIdentifier).IsRequired();
            builder.Property(x => x.ToAccountOwner).IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.TransferActivity).IsRequired(false).IsUnicode(false).HasMaxLength(20);
            builder.Property(x => x.TransferAmount).IsRequired();
            builder.Property(x => x.TransferIdentifier).IsRequired();
            builder.Property(x => x.TransferStatus).IsRequired(false).IsUnicode(false).HasMaxLength(10);
        }
    }
}
