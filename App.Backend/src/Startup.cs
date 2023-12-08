using System.Reflection;
using App.Lib.Configuration;
using App.Lib.Configuration.Options;
using App.Lib.Data;
using GraphQL.AspNet.Configuration;
using GraphQL.AspNet.Security;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace App.Backend;

public class Startup : IDisposable
{
    // Lock used due to GraphQL.AspNet.Configuration.GraphQLSchemaBuilderExtensions.SCHEMA_REGISTRATIONS being static
    private static object _lock = new();

    private IHostEnvironment Environment { get; }

    public Startup(IHostEnvironment environment)
    {
        Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var lockWasTaken = false;
        Monitor.Enter(_lock, ref lockWasTaken);

        services.AddProblemDetails();
        services.AddResponseCompression(options =>
        {
            options.MimeTypes = new[] { "application/json", "application/graphql-response+json" };
        });

        services.AddGraphQL(c =>
        {
            c.AuthorizationOptions.Method = AuthorizationMethod.PerRequest;
            c.ExecutionOptions.ResolverIsolation = ResolverIsolationOptions.All;
            c.ResponseOptions.ExposeExceptions = Environment.IsDevelopment();
            c.ExecutionOptions.DebugMode = Environment.IsDevelopment() || Environment.IsStaging();

            // Register all controllers
            c.AddAssembly(Assembly.GetAssembly(typeof(Startup)));
        });

        var corsPolicy = new CorsPolicyBuilder();
        services.PostConfigure<AppOptions>(appOptions =>
        {
            corsPolicy
                .WithOrigins(appOptions.FrontendUrl.TrimEnd('/'))
                .WithHeaders("Authorization", "Content-Type")
                .WithMethods("GET", "POST");
        });
        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(corsPolicy.Build());
        });

        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddScoped<IOrganisationIdProvider, HttpContextOrganisationIdProvider>();
    }

    public void Configure(IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        var authOptions = app.ApplicationServices.GetRequiredService<IOptions<AuthOptions>>().Value;
        var appOptions = app.ApplicationServices.GetRequiredService<IOptions<AppOptions>>().Value;
        logger.LogInformation("Started application version: {Version} environment: {Environment}",
            appOptions.Version, appOptions.Environment);
        logger.LogInformation("Auth0 {Domain} and audience {Audience}",
            authOptions.Domain, authOptions.Audience);
        logger.LogInformation("Frontend URL {AppFrontendUrl}",
            appOptions.FrontendUrl);

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCompression();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseCors();
        app.UseAppHealthChecks();

        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseGraphQL();
        Unlock();
    }

    public void Dispose()
    {
        Unlock();
    }

    private static void Unlock()
    {
        GraphQLSchemaBuilderExtensions.Clear();
        Monitor.Exit(_lock);
    }
}