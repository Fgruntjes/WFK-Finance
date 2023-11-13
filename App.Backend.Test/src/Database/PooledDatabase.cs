using Microsoft.Extensions.ObjectPool;

namespace App.Backend.Test.Database;

public sealed class PooledDatabase : IAsyncDisposable
{
    private readonly IDatabase _database;

    private readonly ObjectPool<IDatabase> _pool;

    internal PooledDatabase(ObjectPool<IDatabase> pool)
    {
        _pool = pool;
        _database = pool.Get();
    }

    public string ConnectionString => _database.ConnectionString;

    public void EnsureInitialized(IServiceProvider services)
    {
        _database.Initialize(services);
    }

    public async ValueTask DisposeAsync()
    {
        await _database.Clean();
        _pool.Return(_database);
    }
}