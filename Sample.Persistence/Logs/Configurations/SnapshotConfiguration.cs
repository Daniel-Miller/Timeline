
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Persistence.Logs.Stores;

namespace Sample.Persistence.Logs
{
    public class SnapshotConfiguration : IEntityTypeConfiguration<SerializedSnapShot>
    {
        private readonly string _schema;

        public SnapshotConfiguration() : this("logs") { }

        public SnapshotConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<SerializedSnapShot> builder)
        {
            builder.ToTable("Snapshot", _schema);
            builder.HasKey(x => new { x.AggregateIdentifier });

            builder.Property(x => x.AggregateIdentifier).IsRequired();
            builder.Property(x => x.AggregateState).IsRequired().IsUnicode(true);
            builder.Property(x => x.AggregateVersion).IsRequired();
        }
    }
}
