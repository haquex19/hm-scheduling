using Hm.Scheduling.Core.Entities;

namespace Hm.Scheduling.Core.Stores;

public interface IAppointmentAvailabilityStore : IStore<AppointmentAvailability>
{
    Task<List<AppointmentAvailability>> SearchAsync(AppointmentSlotSearchOptions? options = null);

    Task<bool> ExistsAnyOverlappingTimeAsync(DateTimeOffset startTime, DateTimeOffset endTime);

    Task<bool> ExistsByIdAsync(Guid id);
}
