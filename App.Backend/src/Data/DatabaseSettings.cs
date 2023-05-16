using MongoDB.Driver;

namespace App.Backend.Data;

public class DatabaseSettings
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
}