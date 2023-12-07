using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using App.Lib.Data;

namespace App.Data.Migrations;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.local.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433; Database=development; User Id=sa; Password=myLeet123Password!; Encrypt=False";
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer(connectionString, o =>
        {
            o.MigrationsAssembly(typeof(DatabaseContextFactory).Assembly.FullName);
            o.ConfigureDatabaseOptions();
        });

        return new DatabaseContext(optionsBuilder.Options);
    }
}
