using MongoDB.Bson;

namespace App.Backend.Data.Entity;

public interface IEntity
{
    public ObjectId Id { get; }

    public ObjectId TenantId { get; }
}
