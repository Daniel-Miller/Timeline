using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class PersonSummaryConfiguration : IEntityTypeConfiguration<PersonSummary>
    {
        private readonly string _schema;

        public PersonSummaryConfiguration() : this("queries") { }

        public PersonSummaryConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<PersonSummary> builder)
        {
            builder.ToTable("PersonSummary", _schema);
            builder.HasKey(x => new { x.PersonIdentifier });

            builder.Property(x => x.OpenAccountCount).IsRequired();
            builder.Property(x => x.TotalAccountBalance).IsRequired();
            builder.Property(x => x.PersonIdentifier).IsRequired();
            builder.Property(x => x.PersonName).IsRequired(false).IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.PersonRegistered).IsRequired();
            builder.Property(x => x.TenantIdentifier).IsRequired();
        }
    }
}
