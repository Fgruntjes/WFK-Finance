using App.Job.InstitutionAccountTransactionImport;
using App.Lib.Configuration;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .UseConfiguration()
    .UseDataProtection()
    .UseServiceBusListener<InstitutionAccountTransactionImportJob, MessageHandler>();

builder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddSingleton<MessageHandler>();
});

await builder.RunConsoleAsync();