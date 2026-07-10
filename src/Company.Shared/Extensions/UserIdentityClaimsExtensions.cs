using System.Globalization;
using System.Security.Claims;

namespace Company.Shared.Extensions;

public static class UserIdentityClaimsExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal principal, out long userId)
    {
        string value = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(value)
            && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out userId))
            return true;

        userId = 0;
        return false;
    }
}
