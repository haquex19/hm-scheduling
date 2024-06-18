using System.Net;
using AutoMapper;
using FluentValidation;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Queries;

public class GetAppointmentAvailabilityByIdRequest(Guid id) : IRequest<AppointmentAvailabilityModel>
{
    public Guid Id { get; } = id;
}

public class GetAppointmentAvailabilityByIdRequestValidator : AbstractValidator<GetAppointmentAvailabilityByIdRequest>
{
    public GetAppointmentAvailabilityByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetAppointmentAvailabilityByIdRequestHandler(IMapper mapper, IAppointmentAvailabilityStore store)
    : IRequestHandler<GetAppointmentAvailabilityByIdRequest, AppointmentAvailabilityModel>
{
    public async Task<AppointmentAvailabilityModel> Handle(
        GetAppointmentAvailabilityByIdRequest request,
        CancellationToken cancellationToken)
    {
        var availability = await store.FindByIdAsync(request.Id);
        if (availability is null)
        {
            throw new RequestException(HttpStatusCode.NotFound);
        }

        return mapper.Map<AppointmentAvailabilityModel>(availability);
    }
}
