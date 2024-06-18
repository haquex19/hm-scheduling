using Hm.Scheduling.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hm.Scheduling.Infrastructure.Database.EntityTypeConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasIndex(x => x.NormalizedEmail)
            .IsUnique();

        builder
            .Property(x => x.Email)
            .HasMaxLength(512);

        builder
            .Property(x => x.NormalizedEmail)
            .HasMaxLength(512);

        builder
            .Property(x => x.Prefix)
            .HasMaxLength(16);

        builder
            .Property(x => x.FirstName)
            .HasMaxLength(256);

        builder
            .Property(x => x.LastName)
            .HasMaxLength(256);
    }
}
