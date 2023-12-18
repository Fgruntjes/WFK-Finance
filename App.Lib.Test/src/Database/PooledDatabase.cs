using App.Lib.Data;
using Microsoft.Extensions.ObjectPool;

namespace App.Lib.Test.Database;

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

    public void WithData(Action<DatabaseContext> assertAction)
    {
        _database.WithData(assertAction);
    }

    public void SeedData(Action<DatabaseContext> seedAction)
    {
        _database.SeedData(seedAction);
    }
}