namespace Hm.Scheduling.Core.Services;

public interface IHmClock
{
    DateTimeOffset UtcNowOffset();
}
