using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace App.Backend.Data.Entity;

public class BankAccountEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public InstitutionConnectionEntity Connection { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public string BankName { get; init; } = null!;
    public string? AccountName { get; init; }
    public string AccountNumber { get; init; } = null!;

    internal static void EnsureIndexes(IMongoCollection<BankAccountEntity> collection)
    {
        collection.Indexes.CreateOneAsync(new CreateIndexModel<BankAccountEntity>(
            Builders<BankAccountEntity>.IndexKeys.Ascending(e => e.ExternalId),
            new CreateIndexOptions { Unique = true }));
    }
}