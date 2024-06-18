using System.Net;
using AutoMapper;
using FluentValidation;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Stores;
using MediatR;

namespace Hm.Scheduling.Core.Commands;

public class CreateUserRequest : IRequest<UserModel>
{
    public string? Prefix { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public UserType Type { get; set; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Prefix)
            .MaximumLength(16);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(512)
            .EmailAddress();
    }
}

public class CreateUserRequestHandler(IMapper mapper, IUserStore userStore)
    : IRequestHandler<CreateUserRequest, UserModel>
{
    private readonly RequestError _userExistsError = new(
        "UserExists",
        "A user with this email address already exists.");

    public async Task<UserModel> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (await userStore.ExistsByEmailAsync(request.Email))
        {
            throw new RequestException(HttpStatusCode.Conflict, _userExistsError);
        }

        var user = new User
        {
            Prefix = request.Prefix,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Type = request.Type
        };

        await userStore.CreateAsync(user);
        return mapper.Map<UserModel>(user);
    }
}
