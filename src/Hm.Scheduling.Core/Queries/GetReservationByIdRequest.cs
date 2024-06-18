using System.Net;
using AutoMapper;
using FluentValidation;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Queries;

public class GetReservationByIdRequest(Guid id) : IRequest<ReservationModel>
{
    public Guid Id { get; } = id;
}

public class GetReservationByIdRequestValidator : AbstractValidator<GetReservationByIdRequest>
{
    public GetReservationByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetReservationByIdRequestHandler(IMapper mapper, IReservationStore store)
    : IRequestHandler<GetReservationByIdRequest, ReservationModel>
{
    public async Task<ReservationModel> Handle(GetReservationByIdRequest request, CancellationToken cancellationToken)
    {
        var reservation = await store.FindByIdAsync(request.Id);
        if (reservation is null)
        {
            throw new RequestException(HttpStatusCode.NotFound);
        }

        return mapper.Map<ReservationModel>(reservation);
    }
}
