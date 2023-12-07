using Microsoft.Extensions.ObjectPool;
using Respawn;
using Testcontainers.MsSql;
namespace App.Backend.Test.Database;

public sealed class DatabasePoolPolicy : IPooledObjectPolicy<Database>
{
    private readonly MsSqlContainer _container;
    private readonly RespawnerOptions _respawnerOptions;


    public DatabasePoolPolicy(MsSqlContainer container, RespawnerOptions respawnerOptions)
    {
        _container = container;
        _respawnerOptions = respawnerOptions;
    }

    public Database Create() => new(_container, _respawnerOptions);

    public bool Return(Database obj) => true;
}
