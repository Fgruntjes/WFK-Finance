using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data;

public static class DatabaseConfiguration
{
    public static void AddDatabase(
        this IServiceCollection services,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder>? configAction = null)
    {
        services.AddSqlServer<DatabaseContext>(connectionString, optionsBuilder =>
        {
            optionsBuilder.UseNodaTime();
            configAction?.Invoke(optionsBuilder);
        });
    }
}
