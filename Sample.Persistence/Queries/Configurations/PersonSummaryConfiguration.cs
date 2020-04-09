using System.Data.Entity.ModelConfiguration;

using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class PersonSummaryConfiguration : EntityTypeConfiguration<PersonSummary>
    {
        public PersonSummaryConfiguration() : this("queries") { }

        public PersonSummaryConfiguration(string schema)
        {
            ToTable(schema + ".PersonSummary");
            HasKey(x => new { x.PersonIdentifier });

            Property(x => x.OpenAccountCount).IsRequired();
            Property(x => x.TotalAccountBalance).IsRequired();
            Property(x => x.PersonIdentifier).IsRequired();
            Property(x => x.PersonName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.PersonRegistered).IsRequired();
            Property(x => x.TenantIdentifier).IsRequired();
        }
    }
}
