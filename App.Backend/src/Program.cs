using App.Backend.Startup;
using GraphQL.AspNet.Configuration;
using GraphQL.AspNet.Security;
using App.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Sentry;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile("appsettings.local.json", true);

if (!builder.Environment.IsDevelopment())
{
    SentrySdk.Init(options =>
    {
        options.Dsn = builder.Configuration["Sentry:Dsn"];
        options.AutoSessionTracking = true;
    });
}

builder.Services.AddAuth(
    builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Missing 'Auth0:Domain' setting."),
    builder.Configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Missing 'Auth0:Audience' setting.")
);
builder.Services.AddAppDataProtection(
    builder.Configuration["App:KeyVaultUri"] ?? throw new InvalidOperationException("Missing 'App:KeyVaultUri' setting."),
    builder.Configuration["App:KeyName"] ?? throw new InvalidOperationException("Missing 'App:KeyName' setting.")
);
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = new[] { "application/json", "application/graphql-response+json" };
});

var appFrontendUrl = builder.Configuration["App:FrontendUrl"]
        ?? throw new Exception("Missing 'App:FrontendUrl' setting.");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(appFrontendUrl.TrimEnd('/'))
                .WithHeaders("Authorization", "Content-Type")
                .WithMethods("GET", "POST");
        });
});

// App configuration
builder.Services.AddDatabase(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing 'ConnectionStrings:DefaultConnection' setting."));
builder.Services.AddGraphQL(c =>
    {
        c.AuthorizationOptions.Method = AuthorizationMethod.PerRequest;
        c.ExecutionOptions.ResolverIsolation = ResolverIsolationOptions.All;
        c.ResponseOptions.ExposeExceptions = builder.Environment.IsDevelopment();
    });
builder.Services.AddNordigenClient(builder.Configuration);
builder.Services.AddAppServices();

var app = builder.Build();
app.Logger.LogInformation("Started application");
app.Logger.LogInformation("Auth0 {Domain} and audience {Audience}",
    builder.Configuration["Auth0:Domain"],
    builder.Configuration["Auth0:Audience"]);
app.Logger.LogInformation("Frontend URL {AppFrontendUrl}", appFrontendUrl);

app.MapHealthChecks("/.health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("readiness"),
});
app.MapHealthChecks("/.health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("liveness"),
});
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseGraphQL();

app.Run("http://*:8080");

public partial class Program
{

}