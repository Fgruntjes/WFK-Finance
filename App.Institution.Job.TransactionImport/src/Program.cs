using App.Institution.Job.TransactionImport;
using App.Lib.Configuration;
using App.Lib.Data;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.Institution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .UseDatabase()
    .UseConfiguration()
    .UseDataProtection()
    .UseServiceBusListener<TransactionImportJob, MessageHandler>();

builder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddScoped<MessageHandler>();
    serviceCollection.AddScoped<IOrganisationIdProvider, UnimplementedOrganisationIdProvider>();
});

await builder.RunConsoleAsync();