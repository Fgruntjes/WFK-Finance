using App.Backend.Tests.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Backend.Tests;

public class TestApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Production);
        builder.ConfigureTestAuthServices();
        builder.ConfigureServices(services =>
        {
            services.AddLogging(logBuilder => logBuilder.AddConsole());
        });

        return base.CreateHost(builder);
    }
}