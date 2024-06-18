using AutoMapper;
using Hm.Scheduling.Core.Models;

namespace Hm.Scheduling.Core.Entities;

public class Reservation : IEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    public Guid AppointmentAvailabilityId { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }

    public DateTimeOffset? ConfirmedOn { get; set; }

    public User? User { get; set; }

    public AppointmentAvailability? AppointmentAvailability { get; set; }
}

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationModel>();
    }
}
