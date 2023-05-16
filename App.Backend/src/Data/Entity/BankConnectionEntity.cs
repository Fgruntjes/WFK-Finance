using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace App.Backend.Data.Entity;

public class BankConnectionEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId TenantId { get; set; }
    public string ExternalId { get; set; } = null!;
    public string BankId { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;

    internal static void EnsureIndexes(IMongoCollection<BankConnectionEntity> collection)
    {
        collection.Indexes.CreateOneAsync(new CreateIndexModel<BankConnectionEntity>(
            Builders<BankConnectionEntity>.IndexKeys
                .Ascending(e => e.TenantId)
                .Ascending(e => e.BankId),
            new CreateIndexOptions { Unique = true }));
    }
}