using App.Backend.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace App.Backend.Data.Entity;

public class InstitutionConnectionEntity : IListableEntity
{
    public class InstitutionAccount
    {
        public string ExternalId { get; set; } = null!;
        public string? OwnerName { get; init; }
        public string? Iban { get; init; } = null!;
    }

    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId TenantId { get; set; }
    public ObjectId InstitutionId { get; set; }
    public string ExternalConnectionId { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;
    public IList<InstitutionAccount> Accounts { get; set; } = null!;

    public InstitutionConnectionEntity()
    {
        Accounts = new List<InstitutionAccount>();
    }

    internal static void EnsureIndexes(IMongoCollection<InstitutionConnectionEntity> collection)
    {
        collection.Indexes.CreateOneAsync(new CreateIndexModel<InstitutionConnectionEntity>(
            Builders<InstitutionConnectionEntity>.IndexKeys
                .Ascending(e => e.TenantId)
                .Ascending(e => e.ExternalConnectionId),
            new CreateIndexOptions { Unique = true }));
    }
}
