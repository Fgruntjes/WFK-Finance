using System.Net.Http.Headers;
using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Tests;

public class TestFixture<TEntryPoint> :
    IDisposable,
    IClassFixture<TestApplicationFactory<TEntryPoint>> where TEntryPoint : class
{
    protected readonly TestApplicationFactory<TEntryPoint> _factory;
    protected HttpClient _client;

    protected DatabaseContext DatabaseContext => _factory.Services.GetRequiredService<DatabaseContext>();
    protected AuthContext AuthContext => _factory.Services.GetRequiredService<AuthContext>();

    public TestFixture(TestApplicationFactory<TEntryPoint> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthenticationHandler.TestScheme);

        TestUtils.InitVerifyActions();
    }

    public void Dispose()
    {
        TestUtils.VerifyAllSetups();
        ClearDatabase();
    }

    private void ClearDatabase()
    {
        var dbContext = _factory.Services.GetService<DatabaseContext>();
        if (dbContext == null)
        {
            return;
        }

        var database = dbContext.Database;
        var filter = Builders<BsonDocument>.Filter.Not(Builders<BsonDocument>.Filter.Eq("_id", BsonNull.Value));
        var databaseCollections = database.ListCollectionNames().ToEnumerable();
        foreach (var name in databaseCollections)
        {
            database.GetCollection<BsonDocument>(name).DeleteMany(filter);
        }
    }
}