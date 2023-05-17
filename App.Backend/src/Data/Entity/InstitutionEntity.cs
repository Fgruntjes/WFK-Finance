using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace App.Backend.Data.Entity;

public class InstitutionEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Uri? Logo { get; set; }
    public IList<string> Countries { get; set; } = null!;

    public InstitutionEntity()
    {
        Countries = new List<string>();
    }

    internal static void EnsureIndexes(IMongoCollection<InstitutionEntity> collection)
    {
        collection.Indexes.CreateOneAsync(new CreateIndexModel<InstitutionEntity>(
            Builders<InstitutionEntity>.IndexKeys
                .Ascending(e => e.ExternalId),
            new CreateIndexOptions { Unique = true }));
    }
}