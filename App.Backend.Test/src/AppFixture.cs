using Microsoft.Extensions.DependencyInjection;
using App.Backend.Test.Database;
using Microsoft.Extensions.Logging;
using App.Lib.Data;
using App.Lib.Data.Entity;
using Moq;
using Microsoft.AspNetCore.Mvc.Testing;

namespace App.Backend.Test;

public class AppFixture : IAsyncDisposable
{
    public WebApplicationFactory<Program> Application { get; }
    public HttpClient Client => Application.CreateClient();

    public readonly Guid OrganisationId;
    public readonly Guid AltOrganisationId;

    private readonly PooledDatabase _database;
    private readonly ILoggerProvider _loggerProvider;

    public const string TestUserId = "auth0|qm3vehnjqg885o56wa1wc006";
    public const string AltTestUserId = "auth0|pg164es5iwwqrn13qy3pc4ga";

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _database = databasePool.Get();
        _loggerProvider = loggerProvider;

        Application = CreateApp();

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
        var scope = Application.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        assertAction(context);

        // Explicitly dispose to ensure any exception is thrown within the test 
        scope.Dispose();
    }

    public void WithMock<T>(WebApplicationFactory<Program> app, Action<Mock<T>> action)
        where T : class
    {
        var mock = app.Services.GetRequiredService<Mock<T>>();
        action(mock);
    }

    public void WithMock<T>(Action<Mock<T>> action)
        where T : class
    {
        WithMock(Application, action);
    }

    public WebApplicationFactory<Program> CreateApp()
    {
        var app = new ApplicationFactory(_database, _loggerProvider);
        _database.EnsureInitialized();
        return app;
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}