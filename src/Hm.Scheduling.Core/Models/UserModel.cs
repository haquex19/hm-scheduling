using Hm.Scheduling.Core.Entities;

namespace Hm.Scheduling.Core.Models;

public class UserModel
{
    public Guid Id { get; set; }

    public string? Prefix { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public UserType Type { get; set; }
}
