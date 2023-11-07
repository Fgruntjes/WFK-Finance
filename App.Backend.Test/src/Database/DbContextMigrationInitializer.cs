using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Backend.Test.Database;

public sealed class DbContextMigrationInitializer<TDbContext> : IDatabaseInitializer
    where TDbContext : DbContext
{
    public void Initialize(IServiceProvider services)
    {
        using var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.Migrate();
    }
}