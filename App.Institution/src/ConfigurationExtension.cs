using System.Runtime.CompilerServices;
using App.Institution.Interface;
using App.Institution.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using VMelnalksnis.NordigenDotNet.DependencyInjection;

[assembly: InternalsVisibleTo("App.Institution.Test")]

namespace App.Institution;

public static class ConfigurationExtension
{
    public static IHostBuilder UseInstitution(this IHostBuilder host)
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
            services.AddScoped<ITransactionImportService, TransactionImportService>();
            services.AddScoped<ITransactionImportQueueService, TransactionImportQueueService>();
        });
    }

}
