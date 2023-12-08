using App.Lib.Configuration;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .UseConfiguration()
    .UseDataProtection()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddServiceBusListener<InstitutionAccountTransactionImportJob>(
            hostContext.Configuration["ConnectionStrings::ServiceBus"]
                ?? throw new Exception("Missing 'ConnectionStrings::ServiceBus' setting.")
        );
    });

await builder.RunConsoleAsync();