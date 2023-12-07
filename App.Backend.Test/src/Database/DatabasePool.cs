using Microsoft.Extensions.ObjectPool;

namespace App.Backend.Test.Database;

public sealed class DatabasePool
{
    private readonly ObjectPool<Database> _pool;

    public DatabasePool(IPooledObjectPolicy<Database> policy)
    {
        var poolFactory = new DefaultObjectPoolProvider();
        _pool = poolFactory.Create(policy);
    }

    public PooledDatabase Get()
    {
        return new(_pool);
    }
}
