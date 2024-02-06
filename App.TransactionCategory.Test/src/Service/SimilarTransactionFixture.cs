using App.Lib.Data.Entity;
using App.Lib.Test.Data;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;

namespace App.TransactionCategory.Test.Service;

public class SimilarTransactionFixture : AppFixture
{
    public Guid AccountId { get; }
    public Guid AltAccountId { get; }

    public SimilarTransactionFixture(DatabasePool databasePool, ILoggerProvider loggerProvider) : base(databasePool, loggerProvider)
    {
        var account = new FakeInstitutionAccountBuilder(OrganisationId).Generate();
        var altAccount = new FakeInstitutionAccountBuilder(AltOrganisationId).Generate();

        AccountId = account.Id;
        AltAccountId = altAccount.Id;

        Database.SeedData(context =>
        {
            context.Add(account);
            context.Add(altAccount);
            context.AddRange(
                new FakeTransactionCategoryBuilder(OrganisationId).WithDefaults(entity =>
                {
                    entity.Name = "Salary";
                    entity.Group = TransactionCategoryGroup.Income;
                }).Generate(),
                new FakeTransactionCategoryBuilder(OrganisationId).WithDefaults(entity =>
                {
                    entity.Name = "Insurance";
                    entity.Group = TransactionCategoryGroup.Expense;
                }).Generate(),
                new FakeTransactionCategoryBuilder(AltOrganisationId).WithDefaults(entity =>
                {
                    entity.Name = "Salary";
                    entity.Group = TransactionCategoryGroup.Income;
                }).Generate());
        });
    }
}
