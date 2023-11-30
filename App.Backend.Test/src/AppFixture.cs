using App.Data;
using App.Backend.Startup;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Microsoft.Extensions.DependencyInjection;
using App.Backend.Test.Database;
using Microsoft.Extensions.Logging;
using App.Backend.Controllers;
using Moq;
using VMelnalksnis.NordigenDotNet;
using App.Data.Migrations;
using GraphQL.AspNet.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using App.Data.Entity;
using GraphQL.AspNet.Interfaces.Security;
using Microsoft.AspNetCore.Http;

namespace App.Backend.Test;

public class AppFixture : IAsyncDisposable
{
    private readonly PooledDatabase _database;
    private readonly ILoggerProvider _loggerProvider;
    private TestServer<GraphSchema>? _server;
    private static readonly object _buildLock = new();

    public TestServer<GraphSchema> Server
    {
        get
        {
            _server ??= CreateServer();

            return _server;
        }
    }
    public IServiceProvider Services => Server.ServiceProvider;
    public Mock<INordigenClient> NordigenClientMoq { get; private set; }
    public readonly Guid OrganisationId;
    public readonly Guid AltOrganisationId;

    public const string TestUserId = "auth0|qm3vehnjqg885o56wa1wc006";
    public const string AltTestUserId = "auth0|pg164es5iwwqrn13qy3pc4ga";

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _database = databasePool.Get();
        _loggerProvider = loggerProvider;

        NordigenClientMoq = new Mock<INordigenClient>();

        OrganisationId = Guid.NewGuid();
        AltOrganisationId = Guid.NewGuid();
        SeedData(context =>
        {
            context.Organisations.Add(new OrganisationEntity
            {
                Id = OrganisationId,
                Slug = TestUserId,
            });

            context.Organisations.Add(new OrganisationEntity
            {
                Id = AltOrganisationId,
                Slug = AltTestUserId,
            });
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void SeedData(Action<DatabaseContext> seedAction)
    {
        WithData(context =>
        {
            seedAction(context);
            context.SaveChanges();
        });
    }

    public void WithData(Action<DatabaseContext> assertAction)
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        assertAction(context);

        // Explicitly dispose to ensure any exception is thrown within the test 
        scope.Dispose();
    }

    public TestServer<GraphSchema> CreateServer(string? authenticatedUser = TestUserId)
    {
        var builder = new TestServerBuilder<GraphSchema>();

        // Mocks
        builder.AddScoped(_ => NordigenClientMoq.Object);

        // Load database
        builder.RegisterMigrationInitializer<DatabaseContext>();
        builder.AddDatabase(
            _database.ConnectionString,
            databaseOptions =>
            {
                databaseOptions.MigrationsAssembly(typeof(DatabaseContextFactory).Assembly.FullName);
            });

        // Configure logging
        builder.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.Services.AddSingleton(_loggerProvider);
        });

        // App configuration
        builder.AddSingleton<IHostEnvironment>(new HostingEnvironment { EnvironmentName = Environments.Production });
        builder.AddAppServices();
        builder.AddGraphQL(options =>
        {
            options.AddController<InstitutionConnectionGetController>();
            options.AddController<InstitutionConnectionListController>();
            options.AddController<InstitutionConnectionCreateController>();
            options.AddController<InstitutionConnectionDeleteController>();
            options.AddController<InstitutionConnectionRefreshController>();
            options.AddController<InstitutionConnectionExtensionController>();
            options.AddController<InstitutionGetController>();
            options.AddController<InstitutionListController>();
            options.ResponseOptions.ExposeExceptions = true;
            options.ExecutionOptions.ResolverIsolation = ResolverIsolationOptions.All;
        });

        var httpContextAccesorMock = new Mock<IHttpContextAccessor>();
        if (authenticatedUser != null)
        {
            builder.AddSingleton(httpContextAccesorMock.Object);
            builder.UserContext.Authenticate(authenticatedUser);
        }

        // Start server
        TestServer<GraphSchema> app;
        lock (_buildLock)
        {
            app = builder.Build();
        }
        _database.EnsureInitialized(app.ServiceProvider);

        // Since graphql test server does not create a http context, we need to create one manually
        if (authenticatedUser != null)
        {
            httpContextAccesorMock.SetupProperty(a => a.HttpContext, new DefaultHttpContext
            {
                User = app.SecurityContext.DefaultUser,
            });
        }

        return app;
    }
}