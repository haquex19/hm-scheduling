using FluentValidation;

namespace Hm.Scheduling.Core.Extensions;

public static class AvailabilityValidators
{
    public static IRuleBuilderOptions<T, DateTimeOffset> MultipleOf15Minutes<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.Minute % 15 == 0)
            .WithErrorCode("MultipleOf15")
            .WithMessage("The time must have a minute that is a multiple of 15.");
    }

    public static IRuleBuilderOptions<T, DateTimeOffset> ValidDateRange<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder,
        Func<T, DateTimeOffset> endTimeGetter)
    {
        return ruleBuilder
            .Must((request, startTime, _) => startTime < endTimeGetter(request))
            .WithErrorCode("RangeInvalid")
            .WithMessage("The date range is invalid. Ensure the start time is less than the end time.");
    }

    public static IRuleBuilderOptions<T, DateTimeOffset?> ValidNullableDateRange<T>(
        this IRuleBuilder<T, DateTimeOffset?> ruleBuilder,
        Func<T, DateTimeOffset?> endTimeGetter)
    {
        return ruleBuilder
            .Must(
                (request, startTime, _) =>
                {
                    var endTime = endTimeGetter(request);
                    return !startTime.HasValue || !endTime.HasValue || startTime < endTime;
                })
            .WithErrorCode("RangeInvalid")
            .WithMessage("The date range is invalid. Ensure the start time is less than the end time.");
    }
}
