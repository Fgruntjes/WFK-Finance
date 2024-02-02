using App.Lib.Test.Database;
using App.Lib.Data.Entity;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.TransactionCategory.Test.Service;

public class TransactionCategoryFixture : AppFixture
{
    public TransactionCategoryEntity TransactionCategoryEntity { get; }

    public TransactionCategoryFixture(DatabasePool databasePool, ILoggerProvider loggerProvider) : base(databasePool, loggerProvider)
    {
        TransactionCategoryEntity = new TransactionCategoryEntity()
        {
            Id = new Guid("445f33bf-7e12-4472-8fbc-d96f2ab03f4b"),
            CreatedAt = Instant.FromUtc(2021, 1, 1, 0, 0),
            Name = "Groceries",
            OrganisationId = OrganisationId,
            Group = TransactionCategoryGroup.Expense,
        };

        Database.SeedData(context =>
        {
            context.TransactionCategory.Add(TransactionCategoryEntity);
        });
    }
}
