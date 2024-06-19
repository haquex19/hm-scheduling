using System.Net;
using Hm.Scheduling.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Hm.Scheduling.Api.Services;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is RequestException requestException)
        {
            httpContext.Response.StatusCode = (int)requestException.StatusCode;
            if (requestException.RequestErrors.Count > 0)
            {
                await httpContext.Response.WriteAsJsonAsync(requestException.RequestErrors, cancellationToken);
            }
        }
        else
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync("An unexpected error has occurred.", cancellationToken);
        }

        return true;
    }
}
