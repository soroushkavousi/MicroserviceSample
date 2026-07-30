using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Company.ApiGateway.Identity;

public sealed class FakeJwtAuthenticationHandler(UrlEncoder encoder, ILoggerFactory logger,
    IOptionsMonitor<FakeJwtOptions> options)
    : AuthenticationHandler<FakeJwtOptions>(options, logger, encoder)
{
    private const string BearerPrefix = "Bearer ";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        string authorization = authHeader.ToString();
        if (!authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Authorization header must use Bearer scheme."));

        string token = authorization[BearerPrefix.Length..].Trim();
        if (!long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out long userId))
            return Task.FromResult(AuthenticateResult.Fail("Bearer token must be a numeric user id."));

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture))
        ];

        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}