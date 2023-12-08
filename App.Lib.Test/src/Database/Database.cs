using App.Lib.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Lib.Test.Database;

public class Database : Database<DatabaseContext>
{
    public Database(string connectionString) : base(connectionString)
    {
    }

    protected override DatabaseContext CreateContext(DbContextOptions<DatabaseContext> databaseOptions)
    {
        return new DatabaseContext(databaseOptions);
    }
}
