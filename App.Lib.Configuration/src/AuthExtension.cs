using System.Security.Claims;
using App.Lib.Configuration.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace App.Lib.Configuration;

public static class AuthExtension
{
    public static IHostBuilder UseAuth(this IHostBuilder builder)
    {
        var options = builder.UseOptions<AuthOptions>(AuthOptions.Section);
        
        return builder.ConfigureServices((context, services) =>
        {
            IdentityModelEventSource.ShowPII = context.HostingEnvironment.IsDevelopment();

            services
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(bearerOptions =>
                {
                    bearerOptions.Authority = $"https://{options.Domain}/";
                    bearerOptions.Audience = options.Audience;
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });
        });
    }
}