using AutoMapper;
using Hm.Scheduling.Core.Models;

namespace Hm.Scheduling.Core.Entities;

public class AppointmentAvailability : IEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public Guid ProviderId { get; set; }

    public User? Provider { get; set; }

    public List<Reservation> Reservations { get; set; } = [];
}

public class AppointmentAvailabilityProfile : Profile
{
    public AppointmentAvailabilityProfile()
    {
        CreateMap<AppointmentAvailability, AppointmentAvailabilityModel>();
    }
}
