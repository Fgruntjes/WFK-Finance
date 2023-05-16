using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Backend.Data.Entity;

public class TenantEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; } = null!;
    public ObjectId[] Users { get; init; } = Array.Empty<ObjectId>();
}

