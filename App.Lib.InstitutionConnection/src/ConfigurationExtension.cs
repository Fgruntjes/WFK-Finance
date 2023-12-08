using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using VMelnalksnis.NordigenDotNet.DependencyInjection;

namespace App.Lib.InstitutionConnection;

public static class ConfigurationExtension
{
    public static IHostBuilder UseInstitutionConnectionClient(this IHostBuilder host)
    {
        return host.ConfigureServices((hostContext, services) =>
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .RetryAsync(3);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

            services
                .AddNordigenDotNet(hostContext.Configuration)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy);
        });
    }

}
