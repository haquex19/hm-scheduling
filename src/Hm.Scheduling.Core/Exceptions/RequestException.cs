using System.Net;
using Hm.Scheduling.Core.Models;

namespace Hm.Scheduling.Core.Exceptions;

public class RequestException : Exception
{
    public RequestException(HttpStatusCode statusCode, RequestError? requestError = null)
    {
        StatusCode = statusCode;

        if (requestError is not null)
        {
            RequestErrors = [requestError];
        }
    }

    public RequestException(HttpStatusCode statusCode, List<RequestError> requestErrors)
    {
        StatusCode = statusCode;
        RequestErrors = requestErrors;
    }

    public HttpStatusCode StatusCode { get; }

    public List<RequestError> RequestErrors { get; set; } = [];
}
