using Microsoft.Extensions.ObjectPool;

namespace App.Backend.Test.Database;

public sealed class PooledDatabase : IAsyncDisposable
{
    private readonly Database _database;

    private readonly ObjectPool<Database> _pool;

    internal PooledDatabase(ObjectPool<Database> pool)
    {
        _pool = pool;
        _database = pool.Get();
    }

    public string ConnectionString => _database.ConnectionString;

    public void EnsureInitialized()
    {
        _database.Initialize();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.Clean();
        _pool.Return(_database);
    }
}