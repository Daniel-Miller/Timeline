using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Timeline.Commands;

namespace Sample.Persistence.Logs
{
    public class CommandConfiguration : IEntityTypeConfiguration<SerializedCommand>
    {
        private readonly string _schema;

        public CommandConfiguration() : this("logs") { }

        public CommandConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<SerializedCommand> builder)
        {
            builder.ToTable("Command", _schema);
            builder.HasKey(x => new { x.CommandIdentifier });

            builder.Property(x => x.AggregateIdentifier).IsRequired();
            builder.Property(x => x.CommandClass).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.CommandData).IsRequired().IsUnicode(true);
            builder.Property(x => x.CommandIdentifier).IsRequired();
            builder.Property(x => x.CommandType).IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.ExpectedVersion).IsRequired(false);
            builder.Property(x => x.IdentityTenant).IsRequired();
            builder.Property(x => x.IdentityUser).IsRequired();
            builder.Property(x => x.SendCancelled).IsRequired(false);
            builder.Property(x => x.SendCompleted).IsRequired(false);
            builder.Property(x => x.SendError).IsRequired(false).IsUnicode(false);
            builder.Property(x => x.SendScheduled).IsRequired(false);
            builder.Property(x => x.SendStarted).IsRequired(false);
            builder.Property(x => x.SendStatus).IsRequired(false).IsUnicode(false).HasMaxLength(20);
        }
    }
}
