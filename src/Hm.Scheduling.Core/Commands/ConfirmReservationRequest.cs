using System.Net;
using FluentValidation;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Commands;

public class ConfirmReservationRequest : IRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
}

public class ConfirmReservationRequestValidator : AbstractValidator<ConfirmReservationRequest>
{
    public ConfirmReservationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public class ConfirmReservationRequestHandler(IReservationStore reservationStore)
    : IRequestHandler<ConfirmReservationRequest>
{
    private readonly RequestError _reservationExpired = new RequestError(
        "ReservationExpired",
        "The reservation has expired.");

    public async Task Handle(ConfirmReservationRequest request, CancellationToken cancellationToken)
    {
        var reservation = await reservationStore.FindByIdAndUserIdAsync(request.Id, request.UserId);
        if (reservation is null)
        {
            throw new RequestException(HttpStatusCode.NotFound);
        }

        if (DateTimeOffset.UtcNow > reservation.ExpiresOn)
        {
            throw new RequestException(HttpStatusCode.UnprocessableEntity, _reservationExpired);
        }

        reservation.ConfirmedOn = DateTimeOffset.UtcNow;
        await reservationStore.UpdateAsync(reservation);
    }
}
