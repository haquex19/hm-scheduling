using System.Net;
using AutoMapper;
using FluentValidation;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Extensions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Commands;

public class CreateAppointmentAvailabilityRequest : IRequest<AppointmentAvailabilityModel>
{
    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public Guid ProviderId { get; set; }
}

public class CreateAppointmentAvailabilityRequestValidator : AbstractValidator<CreateAppointmentAvailabilityRequest>
{
    public CreateAppointmentAvailabilityRequestValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .MultipleOf15Minutes()
            .ValidDateRange(x => x.EndTime);

        RuleFor(x => x.EndTime)
            .MultipleOf15Minutes();
    }
}

public class CreateAppointmentAvailabilityRequestHandler(IMapper mapper, IAppointmentAvailabilityStore store)
    : IRequestHandler<CreateAppointmentAvailabilityRequest, AppointmentAvailabilityModel>
{
    private readonly RequestError _overlappingAvailabilitiesFound = new(
        "OverlappingAvailabilitiesFound",
        "There exists one or more availabilities that overlap with the provided start and end time.");

    public async Task<AppointmentAvailabilityModel> Handle(
        CreateAppointmentAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        // There is a chance here that the user  is trying to send two simultaneous requests that are overlapping each other.
        // Both requests may succeed because they may have read the database before either availability was persisted.
        // We should use a locking mechanism to avoid this race condition. Refer to the README to find potential solutions.
        if (await store.ExistsAnyOverlappingTimeAsync(request.StartTime, request.EndTime))
        {
            throw new RequestException(HttpStatusCode.UnprocessableEntity, _overlappingAvailabilitiesFound);
        }

        var availability = new AppointmentAvailability
        {
            StartTime = request.StartTime.AtZeroSeconds(),
            EndTime = request.EndTime.AtZeroSeconds(),
            ProviderId = request.ProviderId
        };

        await store.CreateAsync(availability);
        return mapper.Map<AppointmentAvailabilityModel>(availability);
    }
}
