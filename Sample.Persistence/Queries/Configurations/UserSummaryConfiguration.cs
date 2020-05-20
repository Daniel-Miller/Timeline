using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Application.Read;

namespace Sample.Persistence.Queries
{
    public class UserSummaryConfiguration : IEntityTypeConfiguration<UserSummary>
    {
        private readonly string _schema;

        public UserSummaryConfiguration() : this("queries") { }

        public UserSummaryConfiguration(string schema)
        {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<UserSummary> builder)
        {
            builder.ToTable("UserSummary", _schema);
            builder.HasKey(x => new { x.UserIdentifier });

            builder.Property(x => x.LoginName).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.LoginPassword).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.UserIdentifier).IsRequired();
            builder.Property(x => x.UserRegistrationStatus).IsRequired().IsUnicode(false).HasMaxLength(10);
        }
    }
}
