using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hm.Scheduling.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AvailabilitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentAvailabilityModel>> GetByIdAsync(Guid id)
    {
        return await mediator.Send(new GetAppointmentAvailabilityByIdRequest(id));
    }

    [HttpGet("slots")]
    public async Task<ActionResult<List<SlotModel>>> SearchSlotsAsync([FromQuery] SearchSlotsRequest request)
    {
        return await mediator.Send(request);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentAvailabilityModel>> CreateAsync(
        CreateAppointmentAvailabilityRequest request)
    {
        var availability = await mediator.Send(request);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            "Availabilities",
            new
            {
                id = availability.Id
            },
            availability);
    }

    [HttpPost("{id}/reservations")]
    public async Task<ActionResult<ReservationModel>> CreateReservationAsync(CreateReservationRequest request)
    {
        var reservation = await mediator.Send(request);
        return CreatedAtAction(
            nameof(ReservationsController.GetByIdAsync),
            "Reservations",
            new
            {
                id = reservation.Id
            },
            reservation);
    }
}
