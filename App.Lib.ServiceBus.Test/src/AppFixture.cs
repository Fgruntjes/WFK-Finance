using System.Reflection;
using App.Lib.ServiceBus.Test.Messages;
using App.Lib.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;

namespace App.Lib.ServiceBus.Test;

public class AppFixture
{
    public IServiceProvider Services { get; }

    public AppFixture(ILoggerProvider loggerProvider)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:ServiceBus", "inmemory://"},
                });
            })
            .UseServiceBusListener<TestMessage, TestMessageHandler>(new[]
            {
                Assembly.GetAssembly(typeof(TestMessage))
            })
            .ConfigureLogging(loggerProvider)
            .ConfigureServices(services =>
            {
                services.AddSingleton<TestMessageHandler>();
            });

        var host = hostBuilder.Build();
        host.Services.StartRebus();
        Services = host.Services;
    }
}