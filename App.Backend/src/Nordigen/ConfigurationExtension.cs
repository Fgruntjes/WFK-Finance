using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using VMelnalksnis.NordigenDotNet.DependencyInjection;

namespace App.Backend.Nordigen;

public static class ConfigurationExtension
{
    public static void AddNordigenClient(this WebApplicationBuilder builder)
    {
        // Configure API clients
        var retryPolicy = HttpPolicyExtensions
          .HandleTransientHttpError()
          .Or<TimeoutRejectedException>()
          .RetryAsync(3);
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        builder.Services
            .AddNordigenDotNet(builder.Configuration)
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy);
    }
}