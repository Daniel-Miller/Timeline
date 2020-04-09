using System.Data.Entity.ModelConfiguration;

using Timeline.Events;

namespace Sample.Persistence.Logs
{
    public class EventConfiguration : EntityTypeConfiguration<SerializedEvent>
    {
        public EventConfiguration() : this("logs") { }

        public EventConfiguration(string schema)
        {
            ToTable(schema + ".Event");
            HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

            Property(x => x.EventClass).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.EventType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.EventData).IsRequired().IsUnicode(true);
        }
    }
}
