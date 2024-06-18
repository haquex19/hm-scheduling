using AutoMapper;
using FluentValidation;
using Hm.Scheduling.Core.Extensions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Queries;

public class SearchSlotsRequest : IRequest<List<SlotModel>>
{
    public Guid? ProviderId { get; set; }

    public DateTimeOffset? DateRangeStart { get; set; }

    public DateTimeOffset? DateRangeEnd { get; set; }
}

public class SearchSlotsRequestValidator : AbstractValidator<SearchSlotsRequest>
{
    public SearchSlotsRequestValidator()
    {
        RuleFor(x => x.DateRangeStart)
            .ValidNullableDateRange(x => x.DateRangeEnd);
    }
}

public class SearchSlotsRequestHandler(IMapper mapper, IAppointmentAvailabilityStore appointmentAvailabilityStore)
    : IRequestHandler<SearchSlotsRequest, List<SlotModel>>
{
    public async Task<List<SlotModel>> Handle(SearchSlotsRequest request, CancellationToken cancellationToken)
    {
        var availabilities = await appointmentAvailabilityStore.SearchAsync(
            new AppointmentSlotSearchOptions
            {
                ProviderId = request.ProviderId,
                DateRangeStart = request.DateRangeStart,
                DateRangeEnd = request.DateRangeEnd,
                IncludeProvider = true,
                IncludeActiveReservations = true
            });

        var slots = new List<SlotModel>();
        foreach (var availability in availabilities)
        {
            var providerModel = mapper.Map<UserModel>(availability.Provider);

            // Store a reservation index here. This allows us to traverse the reservations in linear time.
            var reservationIndex = 0;
            var startTime = availability.StartTime;

            while (startTime <= availability.EndTime)
            {
                var endTime = startTime.AddMinutes(15);
                var potentialSlotModel = new SlotModel
                {
                    Id = availability.Id,
                    StartTime = startTime,
                    EndTime = endTime,
                    Provider = providerModel
                };

                if (availability.Reservations.Count == 0)
                {
                    slots.Add(potentialSlotModel);
                    startTime = endTime;
                    continue;
                }

                // Ensure we do not display an available slot for an active reservation.
                var reservation = availability.Reservations[reservationIndex];
                if (startTime == reservation.StartTime)
                {
                    startTime = reservation.EndTime;
                    reservationIndex++;
                    continue;
                }

                slots.Add(potentialSlotModel);
                startTime = endTime;
            }
        }

        return slots;
    }
}
