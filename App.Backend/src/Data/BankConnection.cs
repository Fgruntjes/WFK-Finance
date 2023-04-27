using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace App.Backend.Data;

public class BankConnection
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string BankId { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
}