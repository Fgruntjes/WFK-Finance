using MongoDB.Bson;

namespace App.Backend;

public class User
{
    public string Id { get; init; } = null!;
    public string ExternalId { get; init; } = null!;
}
