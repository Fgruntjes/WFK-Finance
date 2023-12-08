using App.DataMigrations;
using App.Lib.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace App.Backend.Test.Database;

public sealed class Database
{
    private readonly RespawnerOptions _respawnerOptions;
    private Respawner? _respawner;
    private bool _initialized;

    public string ConnectionString { get; }

    public Database(MsSqlContainer container, RespawnerOptions respawnerOptions)
    {
        _respawnerOptions = respawnerOptions;
        ConnectionString = $"Server=127.0.0.1,{container.GetMappedPublicPort(1433)};Database={Guid.NewGuid()};User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True";
    }

    public void Initialize()
    {
        if (_initialized) return;

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer(ConnectionString, o =>
        {
            o.MigrationsAssembly(typeof(DatabaseContextFactory).Assembly.FullName);
            o.ConfigureDatabaseOptions();
        });

        var context = new DatabaseContext(optionsBuilder.Options);
        context.Database.Migrate();

        _initialized = true;
    }

    public async ValueTask Clean()
    {
        if (!_initialized) return;

        await using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        _respawner ??= await Respawner.CreateAsync(conn, _respawnerOptions);
        await _respawner.ResetAsync(conn);
    }
}