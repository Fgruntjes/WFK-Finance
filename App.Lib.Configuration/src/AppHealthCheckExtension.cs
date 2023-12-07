using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace App.Lib.Configuration;

public static class AppHealthCheckExtension
{
    public const string ReadinessTag = "readiness";
    public const string LivenessTag = "liveness";

    public static IApplicationBuilder UseAppHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/.health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadinessTag),
        });
        app.UseHealthChecks("/.health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(LivenessTag),
        });

        return app;
    }
}