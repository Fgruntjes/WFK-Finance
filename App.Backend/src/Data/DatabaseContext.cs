using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace App.Backend.Data;

public class DatabaseContext
{
    public IMongoCollection<BankConnection> BankConnections { get; }

    public DatabaseContext(IOptions<DatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Name);

        // Create collections
        BankConnections = database.GetCollection<BankConnection>("BankConnections");

        // Ensure indexes
        BankConnections.Indexes.CreateOneAsync(new CreateIndexModel<BankConnection>(
            Builders<BankConnection>.IndexKeys
                .Ascending(connection => connection.TenantId)
                .Ascending(connection => connection.BankId),
            new CreateIndexOptions { Unique = true }));
    }
}