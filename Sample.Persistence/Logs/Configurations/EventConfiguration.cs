using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timeline.Events;

namespace Sample.Persistence.Logs
{
    public class EventConfiguration : IEntityTypeConfiguration<SerializedEvent>
    {
        private readonly string _schema;
        public EventConfiguration() : this("logs") { }

        public EventConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<SerializedEvent> builder)
        {
            builder.ToTable("Event", _schema);
            builder.HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

            builder.Property(x => x.EventClass).IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.EventType).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.EventData).IsRequired().IsUnicode(true);
        }
    }
}
