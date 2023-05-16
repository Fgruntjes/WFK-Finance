using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace App.Backend.Data.Entity;

public class UserEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ExternalId { get; set; } = null!;

    internal static void EnsureIndexes(IMongoCollection<UserEntity> collection)
    {
        collection.Indexes.CreateOneAsync(new CreateIndexModel<UserEntity>(
            Builders<UserEntity>.IndexKeys
                .Ascending(e => e.ExternalId),
            new CreateIndexOptions { Unique = true }));
    }
}

