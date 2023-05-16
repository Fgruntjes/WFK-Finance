using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Backend.Tests.Auth;
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static readonly string TestScheme = "TestScheme";
    public static readonly string DefaultExternalUserId = "oauth|someuser";

    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, DefaultExternalUserId) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}