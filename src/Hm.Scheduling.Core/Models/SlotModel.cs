namespace Hm.Scheduling.Core.Models;

public class SlotModel
{
    public Guid Id { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public UserModel Provider { get; set; } = default!;
}
