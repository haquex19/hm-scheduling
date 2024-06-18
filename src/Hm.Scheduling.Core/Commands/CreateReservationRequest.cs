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

public class CreateReservationRequest : IRequest<ReservationModel>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset Time { get; set; }
}

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Time)
            .MultipleOf15Minutes();
    }
}

public class CreateReservationRequestHandler(
    IMapper mapper,
    IAppointmentAvailabilityStore appointmentAvailabilityStore,
    IReservationStore reservationStore) : IRequestHandler<CreateReservationRequest, ReservationModel>
{
    private readonly RequestError _notAvailableError = new RequestError(
        "NotAvailable",
        "The requested appointment slot is no longer available.");

    public async Task<ReservationModel> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        if (await appointmentAvailabilityStore.ExistsByIdAsync(request.Id))
        {
            throw new RequestException(HttpStatusCode.NotFound);
        }

        var time = request.Time.AtZeroSeconds();

        // There is a race condition here where two users can be reserving the same slot at the same time. It is possible
        // that both users run this line of code and there are no active slots because both users have not created the entry
        // in the database yet.
        // To mitigate this problem, we can provide a locking mechanism. Refer to the README for possible solutions.
        if (await reservationStore.HasActiveSlotAsync(request.Id, time))
        {
            throw new RequestException(HttpStatusCode.UnprocessableEntity, _notAvailableError);
        }

        var reservation = new Reservation
        {
            AppointmentAvailabilityId = request.Id,
            UserId = request.UserId,
            StartTime = time,
            EndTime = time.AddMinutes(15),
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await reservationStore.CreateAsync(reservation);
        return mapper.Map<ReservationModel>(reservation);
    }
}
