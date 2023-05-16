using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Auth;

public static class ConfigurationExtension
{
    public static void AddAuth0(this WebApplicationBuilder builder, Auth0Options settings)
    {
        builder.Services.AddOptions<Auth0Options>();
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = settings.FullDomain;
                options.Audience = settings.Audience;
            });
        builder.Services.AddAuthorization(options =>
        {
            foreach (var scope in settings.ScopeList)
            {
                options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, settings.FullDomain)));
            }
        });

        builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    }
}