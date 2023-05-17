using MongoDB.Bson;

namespace App.Backend.Data.Entity;

public interface IListableEntity
{
    public ObjectId TenantId { get; }
}
