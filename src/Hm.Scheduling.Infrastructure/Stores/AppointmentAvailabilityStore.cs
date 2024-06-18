using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Stores;
using Hm.Scheduling.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Hm.Scheduling.Infrastructure.Stores;

public class AppointmentAvailabilityStore(HmDbContext context) : BaseStore<AppointmentAvailability>(context),
    IAppointmentAvailabilityStore
{
    public async Task<List<AppointmentAvailability>> SearchAsync(AppointmentSlotSearchOptions? options = null)
    {
        var builder = ApplyIncludes(Table, options);
        if (options?.ProviderId is not null)
        {
            builder = builder.Where(x => x.ProviderId == options.ProviderId);
        }

        if (options?.DateRangeStart is not null)
        {
            builder = builder.Where(x => x.StartTime >= options.DateRangeStart);
        }

        if (options?.DateRangeEnd is not null)
        {
            builder = builder.Where(x => x.EndTime < options.DateRangeEnd);
        }

        return await builder.ToListAsync();
    }

    public async Task<bool> ExistsAnyOverlappingTimeAsync(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        return await Table.AnyAsync(
            x => (startTime >= x.StartTime && startTime <= x.EndTime) ||
                 (endTime >= x.StartTime && endTime <= x.EndTime) ||
                 (startTime < x.StartTime && endTime > x.EndTime));
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await Table.AnyAsync(x => x.Id == id);
    }

    private IQueryable<AppointmentAvailability> ApplyIncludes(
        IQueryable<AppointmentAvailability> builder,
        AppointmentSlotSearchOptions? options)
    {
        if (options is null)
        {
            return builder;
        }

        if (options.IncludeProvider)
        {
            builder = builder.Include(x => x.Provider);
        }

        if (options.IncludeActiveReservations)
        {
            builder = builder.Include(
                x => x
                    .Reservations.Where(y => y.ConfirmedOn != null || DateTimeOffset.UtcNow > y.ExpiresOn)
                    .OrderBy(y => y.StartTime));
        }

        return builder;
    }
}
