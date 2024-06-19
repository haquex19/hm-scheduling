using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hm.Scheduling.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserModel>> RegisterUserAsync(CreateUserRequest request)
    {
        return await mediator.Send(request);
    }
}
