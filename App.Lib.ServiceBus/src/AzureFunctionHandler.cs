using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace App.Lib.ServiceBus;

public static class AzureFunctionHandler
{
    [NoAutomaticTrigger]
    public static void Run(IBinder binder, ILogger log, ServiceBusOptions options)
    {
        var attribute = new ServiceBusTriggerAttribute(options.Queue)
        {
            Connection = ServiceBusOptions.ConnectionStringName
        };
        var message = binder.Bind<string>(attribute);

        log.LogInformation("C# ServiceBus queue trigger function processed message: {Message}", message);
    }
}