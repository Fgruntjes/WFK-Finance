using App.Lib.Test.Database;
using App.Lib.Data.Entity;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryFixture : AppFixture
{
    public TransactionCategoryEntity TransactionCategoryEntity { get; }

    public TransactionCategoryFixture(DatabasePool databasePool, ILoggerProvider loggerProvider) : base(databasePool, loggerProvider)
    {
        TransactionCategoryEntity = new TransactionCategoryEntity()
        {
            CreatedAt = Instant.FromUtc(2021, 1, 1, 0, 0),
            Name = "Groceries",
            OrganisationId = OrganisationId,
            Group = CategoryGroup.Expense,
        };

        Database.SeedData(context =>
        {
            context.TransactionCategory.Add(TransactionCategoryEntity);
        });
    }
}
