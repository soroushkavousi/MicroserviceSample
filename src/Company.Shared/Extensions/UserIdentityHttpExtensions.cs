using System.Globalization;
using Company.Shared.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Company.Shared.Extensions;

public static class UserIdentityHttpExtensions
{
    public static bool TryGetUserId(this HttpContext context, out long userId)
    {
        if (context.Request.Headers.TryGetValue(UserIdentityHeaders.UserId, out StringValues value)
            && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out userId))
            return true;

        userId = 0;
        return false;
    }
}
