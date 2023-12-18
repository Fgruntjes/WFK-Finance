using System.Reflection;
using System.Runtime.CompilerServices;
using App.Lib.Configuration;
using App.Lib.ServiceBus.Messages;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Routing;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

[assembly: InternalsVisibleTo("App.Lib.ServiceBus.Test")]
namespace App.Lib.ServiceBus;

public static class ConfigurationExtension
{
    public static IHostBuilder UseServiceBusListener<TMessage, THandler>(
        this IHostBuilder builder,
        IEnumerable<Assembly?>? messageAssemblies = null)
        where THandler : IMessageHandler<TMessage>
        where TMessage : IMessage
    {
        var queueOptions = builder.UseOptions<ServiceBusOptions>(ServiceBusOptions.Section);

        builder.ConfigureHostConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {$"{ServiceBusOptions.Section}:Queue", GetQueueName<TMessage>()},
            });
        });

        return builder.ConfigureServices((hostContext, services) =>
        {
            var connectionString = GetConnectionString(hostContext.Configuration);
            var queue = GetQueueName<TMessage>();

            ConfigureServices(services);
            services.AddRebusHandler<RebusHandler<TMessage, THandler>>();
            services.AddRebus(rebusConfig =>
            {
                if (IsInMemoryConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), queue));
                }
                else if (IsRabbitMqConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseRabbitMq(connectionString, queue));
                }
                else
                {
                    rebusConfig.Transport(t => t.UseAzureServiceBus(
                        connectionString,
                        queue,
                        GetAzureIdentity(connectionString, queueOptions))
                        .DoNotCreateQueues()
                        .DoNotCheckQueueConfiguration());
                }

                rebusConfig.Routing(RebusRouteConfig(messageAssemblies));
                return rebusConfig;
            });
        });
    }

    public static IHostBuilder UseServiceBusPublisher(this IHostBuilder builder, IEnumerable<Assembly?>? messageAssemblies = null)
    {
        var queueOptions = builder.UseOptions<ServiceBusOptions>(ServiceBusOptions.Section);

        return builder.ConfigureServices((hostContext, services) =>
        {
            var connectionString = GetConnectionString(hostContext.Configuration);

            ConfigureServices(services);
            services.AddRebus(rebusConfig =>
            {
                if (IsInMemoryConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseInMemoryTransportAsOneWayClient(new InMemNetwork()));
                }
                else if (IsRabbitMqConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseRabbitMqAsOneWayClient(connectionString));
                }
                else
                {
                    rebusConfig.Transport(t => t.UseAzureServiceBusAsOneWayClient(
                        connectionString,
                        GetAzureIdentity(connectionString, queueOptions)));
                }

                rebusConfig.Routing(RebusRouteConfig(messageAssemblies));
                return rebusConfig;
            });
        });
    }

    private static string GetQueueName(this MemberInfo type)
    {
        var name = type.Name;
        name = name.EndsWith("Job") ? name[..^3] : name;
        name = name.ToLowerInvariant();
        return name;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IServiceBus, RebusServiceBus>();
    }

    private static TokenCredential? GetAzureIdentity(string connectionString, ServiceBusOptions serviceBusOptions)
    {
        return connectionString.Contains("SharedAccessKeyName")
            ? null
            : new ManagedIdentityCredential(serviceBusOptions.AzureClientId);
    }

    private static bool IsInMemoryConnectionString(string connectionString)
        => connectionString.StartsWith("inmemory://");

    private static bool IsRabbitMqConnectionString(string connectionString)
        => connectionString.StartsWith("amqp://");

    private static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString(ServiceBusOptions.ConnectionStringName)
            ?? throw new Exception("Missing 'ConnectionStrings::ServiceBus' setting.");
    }

    private static string GetQueueName<TMessage>()
    {
        return typeof(TMessage).GetQueueName().ToLowerInvariant();
    }

    private static Action<StandardConfigurer<IRouter>> RebusRouteConfig(IEnumerable<Assembly?>? messageAssemblies = null)
    {
        var defaultAssemblies = new[]
        {
            Assembly.GetAssembly(typeof(IMessage)),
            Assembly.GetCallingAssembly(),
        };

        var types = defaultAssemblies
            .Concat(messageAssemblies ?? Array.Empty<Assembly>())
            .Where(a => a != null)
            .Cast<Assembly>()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass)
            .Where(typeof(IMessage).IsAssignableFrom)
            .ToHashSet()
            .ToList();

        return router =>
        {
            var typeBasedRouter = router.TypeBased();
            foreach (var type in types)
            {
                typeBasedRouter.Map(type, GetQueueName(type));
            }
        };
    }
}
