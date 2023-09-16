using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using VMelnalksnis.NordigenDotNet.DependencyInjection;

namespace App.Backend.Startup;

public static class NordigenClient
{
	public static void AddNordigenClient(this IServiceCollection services, IConfiguration configuration)
	{
		// GraphQL configuration
		var retryPolicy = HttpPolicyExtensions
		  .HandleTransientHttpError()
		  .Or<TimeoutRejectedException>()
		  .RetryAsync(3);
		var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);
		services
			.AddNordigenDotNet(configuration)
			.AddPolicyHandler(retryPolicy)
			.AddPolicyHandler(timeoutPolicy);
	}
}