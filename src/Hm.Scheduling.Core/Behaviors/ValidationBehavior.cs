using System.Net;
using FluentValidation;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Hm.Scheduling.Core.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is null)
        {
            return await next();
        }

        var result = await validator.ValidateAsync(request, cancellationToken);
        if (result.IsValid)
        {
            return await next();
        }

        var requestErrors = result
            .Errors.Select(x => new RequestError(x.ErrorCode, x.ErrorMessage))
            .ToList();

        throw new RequestException(HttpStatusCode.BadRequest, requestErrors);
    }
}
