using System.Reflection;
using System.Runtime.CompilerServices;
using App.Lib.Configuration;
using App.Lib.ServiceBus.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

[assembly: InternalsVisibleTo("App.Lib.InstitutionConnection.Test")]
namespace App.Lib.ServiceBus;

public static class ConfigurationExtension
{
    // In memory network used for tests
    private static InMemNetwork? _inMemNetwork;
    private static InMemNetwork InMemNetwork => _inMemNetwork ??= new InMemNetwork(true);

    public static IHostBuilder UseServiceBusListener<TMessage, THandler>(this IHostBuilder builder)
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

            services.AddRebusHandler<RebusHandler<TMessage, THandler>>();
            services.AddRebus(rebusConfig =>
            {
                if (IsInMemoryConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseInMemoryTransport(InMemNetwork, queue));
                }
                else if (IsRabbitMqConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseRabbitMq(connectionString, queue));
                }
                else
                {
                    rebusConfig.Transport(t => t.UseAzureServiceBus(connectionString, queue));
                }

                return rebusConfig;
            }, onCreated: bus => bus.Subscribe<TMessage>());
        });
    }

    public static IHostBuilder UseServiceBusPublisher(this IHostBuilder host)
    {
        return host.ConfigureServices((hostContext, services) =>
        {
            var connectionString = GetConnectionString(hostContext.Configuration);

            services.AddSingleton<IServiceBus, RebusServiceBus>();
            services.AddRebus(rebusConfig =>
            {
                if (IsInMemoryConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseInMemoryTransportAsOneWayClient(InMemNetwork));
                }
                else if (IsRabbitMqConnectionString(connectionString))
                {
                    rebusConfig.Transport(t => t.UseRabbitMqAsOneWayClient(connectionString));
                }
                else
                {
                    rebusConfig.Transport(t => t.UseAzureServiceBusAsOneWayClient(connectionString));
                }

                rebusConfig.Routing(r =>
                {
                    var assembly = Assembly.GetAssembly(typeof(IMessage));
                    if (assembly == null)
                    {
                        throw new Exception("Could not find assembly containing IMessage.");
                    }

                    var types = assembly.GetTypes()
                        .Where(t => t.IsInstanceOfType(typeof(IMessage)));
                    foreach (var type in types)
                    {
                        r.TypeBased().Map(type, GetQueueName(type));
                    }
                });

                return rebusConfig;
            });
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
}
