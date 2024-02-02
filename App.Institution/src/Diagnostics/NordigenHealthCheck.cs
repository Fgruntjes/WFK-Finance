
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace App.Institution.Diagnostics;

class NordigenHealthCheck : IHealthCheck
{
    private readonly HttpClient _nordigenClient;

    public NordigenHealthCheck(IHttpClientFactory clientFactory)
    {
        _nordigenClient = clientFactory.CreateClient("Nordigen");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await _nordigenClient.GetAsync("/");
        if (response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Healthy();
        }
        return HealthCheckResult.Degraded();
    }
}