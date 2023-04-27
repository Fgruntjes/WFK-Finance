using MongoDB.Driver;

namespace App.Backend.Data;

public class DatabaseSettings
{
    public string ConnectionString { get; init; } = null!;
    public string Name { get; init; } = null!;
}