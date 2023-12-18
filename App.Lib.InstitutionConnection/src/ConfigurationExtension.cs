using System.Runtime.CompilerServices;
using App.Lib.InstitutionConnection.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using VMelnalksnis.NordigenDotNet.DependencyInjection;

[assembly: InternalsVisibleTo("App.Lib.InstitutionConnection.Test")]
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

            services.AddScoped<IInstitutionConnectionCreateService, InstitutionConnectionCreateService>();
            services.AddScoped<IInstitutionConnectionRefreshService, InstitutionConnectionRefreshService>();
            services.AddScoped<IInstitutionSearchService, InstitutionSearchService>();
        });
    }

}
