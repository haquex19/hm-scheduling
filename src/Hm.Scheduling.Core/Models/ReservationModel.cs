namespace Hm.Scheduling.Core.Models;

public class ReservationModel
{
    public Guid Id { get; set; }

    public Guid AppointmentAvailabilityId { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }

    public DateTimeOffset? ConfirmedOn { get; set; }
}
