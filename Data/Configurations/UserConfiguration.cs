using Features.User.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.GoogleId).HasColumnType("varchar(255)").IsRequired();
            builder.Property(x => x.Email).HasColumnType("varchar(255)").IsRequired();
            builder.Property(x => x.DisplayName).HasColumnType("varchar(100)").IsRequired();

            builder.HasIndex(x => x.GoogleId).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.DisplayName).IsUnique();
        }
    }
}
