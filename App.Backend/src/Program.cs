using App.Backend;
using App.Lib.Configuration;
using App.Lib.Data;
using App.Lib.InstitutionConnection;

var builder = Host.CreateDefaultBuilder(args);
builder.UseConfiguration();
builder.UseDataProtection();
builder.UseInstitutionConnectionClient();
builder.UseAuth();
builder.UseDatabase();
builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
    webBuilder.UseUrls("http://*:8080");
});

await builder
    .Build()
    .RunAsync();

public partial class Program { }