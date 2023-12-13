using System.Reflection;
using System.Runtime.CompilerServices;
using App.Lib.Configuration;
using App.Lib.ServiceBus.Messages;
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
        builder.UseOptions<ServiceBusOptions>(ServiceBusOptions.Section);

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

            services.AddSingleton<IServiceBus, RebusServiceBus>();
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
                    rebusConfig.Transport(t => t.UseAzureServiceBus(connectionString, queue));
                }

                rebusConfig.Routing(RebusRouteConfig(messageAssemblies));
                return rebusConfig;
            },
                onCreated: bus => bus.Subscribe<TMessage>());
        });
    }

    public static IHostBuilder UseServiceBusPublisher(this IHostBuilder host, IEnumerable<Assembly?>? messageAssemblies = null)
    {
        return host.ConfigureServices((hostContext, services) =>
        {
            var connectionString = GetConnectionString(hostContext.Configuration);

            services.AddSingleton<IServiceBus, RebusServiceBus>();
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
                    rebusConfig.Transport(t => t.UseAzureServiceBusAsOneWayClient(connectionString));
                }

                rebusConfig.Routing(RebusRouteConfig(messageAssemblies));
                return rebusConfig;
            }, isDefaultBus: true);
        });
    }

    public static string GetQueueName(this MemberInfo type)
    {
        var name = type.Name;
        return name.EndsWith("Job") ? name[..^3] : name;
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
        return typeof(TMessage).GetQueueName();
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
