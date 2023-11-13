using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Respawn;
using Testcontainers.MsSql;

namespace App.Backend.Test.Database;

public static class ServiceCollectionExtensions
{
    public static void RegisterMigrationInitializer<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddTransient<IDatabaseInitializer, DbContextMigrationInitializer<TContext>>();
    }

    public static void RegisterSharedDatabaseServices(this IServiceCollection services)
    {
        services.AddSingleton<DatabasePool>();
    }

    public static void RegisterDatabaseContainer(this IServiceCollection services)
    {
        services.RegisterSharedDatabaseServices();
        services.AddTransient(c => new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer
        });
        services.AddTransient<IPooledObjectPolicy<IDatabase>, MsSqlDatabasePoolPolicy>();

        var container = new MsSqlBuilder().Build();
        Utils.RunWithoutSynchronizationContext(() =>
        {
            container.StartAsync().Wait();
        });
        services.AddSingleton(container);
    }
}
