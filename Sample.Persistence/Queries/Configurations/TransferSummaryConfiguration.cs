using System.Data.Entity.ModelConfiguration;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class TransferSummaryConfiguration : EntityTypeConfiguration<TransferSummary>
    {
        public TransferSummaryConfiguration() : this("queries") { }

        public TransferSummaryConfiguration(string schema)
        {
            ToTable(schema + ".TransferSummary");
            HasKey(x => new { x.TransferIdentifier });

            Property(x => x.FromAccountIdentifier).IsRequired();
            Property(x => x.FromAccountOwner).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.TenantIdentifier).IsRequired();
            Property(x => x.ToAccountIdentifier).IsRequired();
            Property(x => x.ToAccountOwner).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.TransferActivity).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.TransferAmount).IsRequired();
            Property(x => x.TransferIdentifier).IsRequired();
            Property(x => x.TransferStatus).IsOptional().IsUnicode(false).HasMaxLength(10);
        }
    }
}
