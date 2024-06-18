using AutoMapper;
using Hm.Scheduling.Core.Models;

namespace Hm.Scheduling.Core.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    public string? Prefix { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string NormalizedEmail { get; set; } = default!;

    public UserType Type { get; set; }

    public List<AppointmentAvailability> Availabilities { get; set; } = [];

    public List<Reservation> Reservations { get; set; } = [];
}

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModel>();
    }
}
