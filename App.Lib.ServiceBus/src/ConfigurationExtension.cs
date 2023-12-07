using System.Reflection;
using App.Lib.ServiceBus.Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;

namespace App.Lib.ServiceBus;

public static class ConfigurationExtension
{
    public static IServiceCollection AddServiceBusPublisher(this IServiceCollection services, string connectionString)
    {
        services.AddRebus(configure =>
            configure.Transport(t => t.UseAzureServiceBusAsOneWayClient(connectionString)));

        return services;
    }

    public static IServiceCollection AddServiceBusListener<TMessage>(this IServiceCollection services, string connectionString)
    {
        var queue = GetQueueName<TMessage>();

        services.AddRebus(configure => configure
            .Transport(t => t.UseAzureServiceBus(connectionString, queue)),
            onCreated: async bus =>
            {
                await bus.Subscribe<TMessage>();
            });

        return services;
    }

    private static string GetQueueName<TMessage>()
    {
        var attribute = typeof(TMessage).GetCustomAttribute<MessageQueueAttribute>()
            ?? throw new Exception("Queue attribute not set for {Type}")
            {
                Data = { { "Type", typeof(TMessage).Name } }
            };

        return attribute.QueueName;
    }
}
