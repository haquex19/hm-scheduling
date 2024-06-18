using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Stores;
using Hm.Scheduling.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Hm.Scheduling.Infrastructure.Stores;

public class ReservationStore(HmDbContext context) : BaseStore<Reservation>(context), IReservationStore
{
    public async Task<Reservation?> FindByIdAndUserIdAsync(Guid id, Guid userId)
    {
        return await Table.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }

    public async Task<bool> HasActiveSlotAsync(Guid availabilityId, DateTimeOffset startTime)
    {
        return await Table.AnyAsync(
            x => x.AppointmentAvailabilityId == availabilityId &&
                 x.StartTime == startTime &&
                 (x.ConfirmedOn != null || DateTimeOffset.UtcNow <= x.ExpiresOn));
    }
}
