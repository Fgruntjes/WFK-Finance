using System.Security.Claims;
using GraphQL.AspNet.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Backend.Startup;

public static class Auth
{
	public static void AddAuth(this IServiceCollection services, string domain)
	{
		services.AddAuthorization()
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = $"https://{domain}/"; ;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = ClaimTypes.NameIdentifier
				};
			});
	}
}