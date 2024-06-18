using Hm.Scheduling.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hm.Scheduling.Infrastructure.Database.EntityTypeConfigurations;

public class ReservationEntityTypeConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder
            .HasIndex(
                x => new
                {
                    x.AppointmentAvailabilityId,
                    x.UserId,
                    x.StartTime
                })
            .IsUnique();
    }
}
