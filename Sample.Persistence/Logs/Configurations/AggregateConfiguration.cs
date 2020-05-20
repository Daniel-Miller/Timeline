using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Timeline.Events;

namespace Sample.Persistence.Logs
{
    public class AggregateConfiguration : IEntityTypeConfiguration<SerializedAggregate>
    {
        private readonly string _schema;

        public AggregateConfiguration() : this("logs") { }

        public AggregateConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<SerializedAggregate> builder)
        {
            builder.ToTable("Aggregate", _schema);
            builder.HasKey(x => new { x.AggregateIdentifier });

            builder.Property(x => x.AggregateClass).IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.AggregateExpires).IsRequired(false);
            builder.Property(x => x.AggregateIdentifier).IsRequired();
            builder.Property(x => x.AggregateType).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.TenantIdentifier).IsRequired();
        }
    }
}
