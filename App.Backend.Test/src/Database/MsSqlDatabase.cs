using Microsoft.Data.SqlClient;
using Respawn;
using Testcontainers.MsSql;

namespace App.Backend.Test.Database;

public sealed class MsSqlDatabase : IDatabase
{
    private readonly IDatabaseInitializer _databaseInitializer;
    private readonly RespawnerOptions _respawnerOptions;
    private Respawner? _respawner;
    private bool _initialized;

    public string ConnectionString { get; }

    public MsSqlDatabase(MsSqlContainer container, IDatabaseInitializer databaseInitializer, RespawnerOptions respawnerOptions)
    {
        _databaseInitializer = databaseInitializer;
        _respawnerOptions = respawnerOptions;
        ConnectionString = $"Server=127.0.0.1,{container.GetMappedPublicPort(1433)};Database={Guid.NewGuid()};User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True";
    }

    public void Initialize(IServiceProvider services)
    {
        if (!_initialized)
        {
            _databaseInitializer.Initialize(services);
            _initialized = true;
        }
    }

    public async ValueTask Clean()
    {
        if (_initialized)
        {
            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            _respawner ??= await Respawner.CreateAsync(conn, _respawnerOptions);
            await _respawner.ResetAsync(conn);
        }
    }
}