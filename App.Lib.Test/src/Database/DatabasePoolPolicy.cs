using Microsoft.Extensions.ObjectPool;
using Testcontainers.MsSql;

namespace App.Lib.Test.Database;

public sealed class DatabasePoolPolicy : IPooledObjectPolicy<Database>
{
    private readonly MsSqlContainer _container;


    public DatabasePoolPolicy(MsSqlContainer container)
    {
        _container = container;
    }

    public Database Create() => new(string.Join(
        ';',
        $"Server=127.0.0.1,{_container.GetMappedPublicPort(1433)}",
        $"Database={Guid.NewGuid()}",
        "User Id=sa",
        "Password=yourStrong(!)Password;TrustServerCertificate=True"));

    public bool Return(Database obj) => true;
}
