namespace Hm.Scheduling.Core.Models;

public class AppointmentAvailabilityModel
{
    public Guid Id { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public Guid ProviderId { get; set; }
}
