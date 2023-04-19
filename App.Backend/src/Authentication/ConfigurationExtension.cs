using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Backend.Authentication;

public static class ConfigurationExtension
{
    public static void AddAuth0(this WebApplicationBuilder builder)
    {
        var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {

            options.Authority = domain;
            options.Audience = builder.Configuration["Auth0:Audience"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });
        builder.Services.AddAuthorization(options =>
        {
            var scopes = builder.Configuration["Auth0:Scope"]?.Split(' ');
            if (scopes == null) return;

            foreach (var scope in scopes)
            {
                options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, domain)));
            }
        });
    }
}