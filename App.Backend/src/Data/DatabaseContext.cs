using System.Text.RegularExpressions;
using App.Backend.Data.Entity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace App.Backend.Data;

public class DatabaseContext
{
    private readonly MongoClient _client;
    public readonly IMongoDatabase Database;
    public IMongoCollection<InstitutionConnectionEntity> InstitutionConnections { get; }
    public IMongoCollection<InstitutionEntity> Institutions { get; }
    public IMongoCollection<TenantEntity> Tenants { get; }
    public IMongoCollection<UserEntity> Users { get; }

    public DatabaseContext(IOptions<DatabaseSettings> options)
    {
        var settings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(settings);
        Database = _client.GetDatabase(options.Value.DatabaseName);

        // Create collections
        InstitutionConnections = GetCollection<InstitutionConnectionEntity>();
        Institutions = GetCollection<InstitutionEntity>();
        Tenants = GetCollection<TenantEntity>();
        Users = GetCollection<UserEntity>();

        // Ensure indexes
        InstitutionConnectionEntity.EnsureIndexes(InstitutionConnections);
        InstitutionEntity.EnsureIndexes(Institutions);
        UserEntity.EnsureIndexes(Users);
    }

    internal Task<IClientSessionHandle> StartSessionAsync()
    {
        return _client.StartSessionAsync();
    }

    private IMongoCollection<TEntity> GetCollection<TEntity>()
    {
        var collectionName = Regex.Replace(typeof(TEntity).Name, "Entity$", "");

        return Database.GetCollection<TEntity>(collectionName);
    }
}