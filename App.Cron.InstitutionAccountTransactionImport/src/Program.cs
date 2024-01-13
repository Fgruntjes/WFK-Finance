using App.Lib.Configuration;
using App.Lib.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .UseDatabase()
    .UseConfiguration()
    .UseDataProtection();

builder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddScoped<IOrganisationIdProvider, UnimplementedOrganisationIdProvider>();
});

await builder.RunConsoleAsync();