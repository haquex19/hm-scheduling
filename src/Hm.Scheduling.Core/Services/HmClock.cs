namespace Hm.Scheduling.Core.Services;

public class HmClock : IHmClock
{
    public DateTimeOffset UtcNowOffset()
    {
        return DateTimeOffset.UtcNow;
    }
}
