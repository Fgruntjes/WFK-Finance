using App.DataMigrations;
using App.Lib.Data;
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace App.Lib.Test.Database;

public abstract class Database<TContext>
    where TContext : DbContext
{
    public string ConnectionString { get; }

    private readonly DbContextOptions<TContext> _databaseOptions;
    private bool _initialized;

    public Database(string connectionString)
    {
        ConnectionString = connectionString;
        _databaseOptions = CreateDatabaseOptions();
    }

    protected abstract TContext CreateContext(DbContextOptions<TContext> databaseOptions);

    public void SeedData(Action<TContext> seedAction)
    {
        WithData(context =>
        {
            seedAction(context);
            context.SaveChanges();
        });
    }

    public void WithData(Action<TContext> assertAction)
    {
        var context = CreateContext(_databaseOptions);
        assertAction(context);
        context.Dispose();
    }

    public async Task WithDataAsync(Func<TContext, Task> assertAction)
    {
        var context = CreateContext(_databaseOptions);
        await assertAction(context);
        await context.DisposeAsync();
    }

    public void Initialize()
    {
        if (_initialized) return;

        WithData(context =>
        {
            context.Database.Migrate();
        });

        _initialized = true;
    }

    public async ValueTask Clean()
    {
        if (!_initialized) return;

        await WithDataAsync(async context =>
        {
            var connection = context.Database.GetDbConnection();
            await context.Database.OpenConnectionAsync();

            try
            {
                var respawner = await Respawner.CreateAsync(connection);
                await respawner.ResetAsync(connection);
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }

        });
    }

    private DbContextOptions<TContext> CreateDatabaseOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseSqlServer(ConnectionString, o =>
        {
            o.MigrationsAssembly(typeof(DatabaseContextFactory).Assembly.FullName);
            o.ConfigureDatabaseOptions();
        });

        return optionsBuilder.Options;
    }
}