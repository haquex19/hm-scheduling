namespace Hm.Scheduling.Core.Stores;

public class AppointmentSlotSearchOptions : AppointmentSlotIncludes
{
    public Guid? ProviderId { get; set; }

    public DateTimeOffset? DateRangeStart { get; set; }

    public DateTimeOffset? DateRangeEnd { get; set; }
}
