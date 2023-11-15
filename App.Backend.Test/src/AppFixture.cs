using App.Data;
using App.Backend.Startup;
using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Core;
using App.Backend.Test.Database;
using Microsoft.Extensions.Logging;
using App.Backend.Controllers;
using Moq;
using VMelnalksnis.NordigenDotNet;
using App.Data.Migrations;
using GraphQL.AspNet.Configuration;

namespace App.Backend.Test;

public class AppFixture : IAsyncDisposable
{
    private readonly ISnapshotFullNameReader _testNameResolver;
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
    public Guid OrganisationId => Services.GetRequiredService<OrganisationIdProvider>().OrganisationId;

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _testNameResolver = new XunitSnapshotFullNameReader();
        _database = databasePool.Get();
        _loggerProvider = loggerProvider;

        NordigenClientMoq = new Mock<INordigenClient>();

        SeedData(context =>
        {
            var orgId = Server.ServiceProvider.GetRequiredService<OrganisationIdProvider>().OrganisationId;
            context.Organisations.Add(new Data.Entity.OrganisationEntity
            {
                Id = orgId,
                Slug = "single-organisation"
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
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        assertAction(context);
    }

    public async Task<string> ExecuteQuery(string? queryFile, object? variables = null)
    {
        var context = BuildQueryContext(queryFile, variables);
        return await Server.RenderResult(context);
    }

    public async Task<string> ExecuteQuery(object? variables = null)
    {
        var context = BuildQueryContext(null, variables);
        return await Server.RenderResult(context);
    }

    private TestServer<GraphSchema> CreateServer()
    {
        var builder = new TestServerBuilder<GraphSchema>();

        // Mocks
        builder.AddScoped((_) => NordigenClientMoq.Object);

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

        // TODO handle non authorized test cases
        builder.UserContext.Authenticate();

        // Start server
        TestServer<GraphSchema> app;
        lock (_buildLock)
        {
            app = builder.Build();
        }
        _database.EnsureInitialized(app.ServiceProvider);

        return app;
    }

    private QueryExecutionContext BuildQueryContext(string? queryFile, object? variables = null)
    {
        var queryText = LoadQueryFile(queryFile);
        var queryBuilder = Server.CreateQueryContextBuilder();
        queryBuilder.AddQueryText(queryText);

        if (variables != null)
        {
            queryBuilder.AddVariableData(variables);
        }

        return queryBuilder.Build();
    }

    private string LoadQueryFile(string? queryFile)
    {
        var queryFullName = _testNameResolver.ReadSnapshotFullName();
        queryFile ??= Path.Combine(queryFullName.FolderPath, "__graphql__", queryFullName.Filename + ".graphql");
        if (!File.Exists(queryFile))
        {
            File.Create(queryFile).Dispose();
        }

        var queryText = File.ReadAllText(queryFile);
        return queryText;
    }
}