using Hm.Scheduling.Core.Models;

namespace Hm.Scheduling.Core.Constants;

public static class CommonRequestErrors
{
    public static readonly RequestError IdAndLocationMismatch = new(
        "IdLocationMismatch",
        "The id in the request body does not match the id of the resource at this location.");
}
