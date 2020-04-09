using System.Data.Entity.ModelConfiguration;

using Timeline.Commands;

namespace Sample.Persistence.Logs
{
    public class CommandConfiguration : EntityTypeConfiguration<SerializedCommand>
    {
        public CommandConfiguration() : this("logs") { }

        public CommandConfiguration(string schema)
        {
            ToTable(schema + ".Command");
            HasKey(x => new { x.CommandIdentifier });

            Property(x => x.AggregateIdentifier).IsRequired();
            Property(x => x.CommandClass).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CommandData).IsRequired().IsUnicode(true);
            Property(x => x.CommandIdentifier).IsRequired();
            Property(x => x.CommandType).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ExpectedVersion).IsOptional();
            Property(x => x.IdentityTenant).IsRequired();
            Property(x => x.IdentityUser).IsRequired();
            Property(x => x.SendCancelled).IsOptional();
            Property(x => x.SendCompleted).IsOptional();
            Property(x => x.SendError).IsOptional().IsUnicode(false);
            Property(x => x.SendScheduled).IsOptional();
            Property(x => x.SendStarted).IsOptional();
            Property(x => x.SendStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
        }
    }
}
