namespace Hm.Scheduling.Core.Extensions;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset AtZeroSeconds(this DateTimeOffset dateTimeOffset)
    {
        return new DateTimeOffset(
            dateTimeOffset.Year,
            dateTimeOffset.Month,
            dateTimeOffset.Day,
            dateTimeOffset.Hour,
            dateTimeOffset.Minute,
            0,
            dateTimeOffset.Offset);
    }
}
