using System.Net;
using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Constants;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hm.Scheduling.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ReservationModel> GetByIdAsync(Guid id)
    {
        return await mediator.Send(new GetReservationByIdRequest(id));
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmAsync(Guid id, ConfirmReservationRequest request)
    {
        if (id != request.Id)
        {
            throw new RequestException(HttpStatusCode.BadRequest, CommonRequestErrors.IdAndLocationMismatch);
        }

        await mediator.Send(request);
        return NoContent();
    }
}
