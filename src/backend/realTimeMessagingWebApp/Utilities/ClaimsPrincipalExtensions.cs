using System.Security.Claims;

namespace realTimeMessagingWebApp.Utilities;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdString = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        return Guid.Parse(userIdString!);
    }
}
