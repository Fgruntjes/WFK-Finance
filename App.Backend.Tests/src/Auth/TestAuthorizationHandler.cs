using App.Backend.Auth;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Tests.Auth;

public class TestAuthorizationHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasScopeRequirement requirement
    )
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}