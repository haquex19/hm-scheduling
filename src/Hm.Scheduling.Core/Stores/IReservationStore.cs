using Hm.Scheduling.Core.Entities;

namespace Hm.Scheduling.Core.Stores;

public interface IReservationStore : IStore<Reservation>
{
    Task<Reservation?> FindByIdAndUserIdAsync(Guid id, Guid userId);

    Task<bool> HasActiveSlotAsync(Guid availabilityId, DateTimeOffset startTime);
}
