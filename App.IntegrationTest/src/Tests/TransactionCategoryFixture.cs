using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.IntegrationTest.Tests;

public class TransactionCategoryFixture<TestType> : AppFixture<TestType>, IAsyncLifetime
{
    private const string _appTestInstitutionId = "TRANSACTION_CATEGORY_TEST_INSTITUTION";

    public TransactionCategoryFixture(ILoggerProvider loggerProvider, TestContext testContext) : base(loggerProvider, testContext)
    {

    }

    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Inject test transactions
        Database.SeedData(context =>
        {
            // Delete old data
            context.InstitutionAccountTransactions
                .Where(e => e.Account.InstitutionConnection.Institution.ExternalId == _appTestInstitutionId)
                .ExecuteDelete();
            context.InstitutionAccounts
                .Where(e => e.InstitutionConnection.Institution.ExternalId == _appTestInstitutionId)
                .ExecuteDelete();
            context.InstitutionConnections
                .Where(e => e.Institution.ExternalId == _appTestInstitutionId)
                .ExecuteDelete();
            context.Institutions
                .Where(e => e.ExternalId == _appTestInstitutionId)
                .ExecuteDelete();
            context.TransactionCategory
                .Where(e => e.OrganisationId == OrganisationId)
                .ExecuteDelete();

            // Inject new
            var institution = new InstitutionEntity
            {
                ExternalId = _appTestInstitutionId,
                Name = "Transaction category test institution",
                CountryIso2 = "DE",
            };
            var institutionConnection = new InstitutionConnectionEntity
            {
                InstitutionId = institution.Id,
                ConnectUrl = new Uri("https://example.com"),
                ExternalId = "TRANSACTION_CATEGORY_TEST_CONNECTION",
                OrganisationId = OrganisationId,
            };
            var institutionAccount = new InstitutionAccountEntity
            {
                InstitutionConnectionId = institutionConnection.Id,
                Iban = "DE89370400440532013000",
                ExternalId = "TRANSACTION_CATEGORY_TEST_ACCOUNT",
            };

            var accountTransactionA = new InstitutionAccountTransactionEntity
            {
                AccountId = institutionAccount.Id,
                TransactionCode = "TEST_TRANSACTION",
                Amount = 100,
                Currency = "EUR",
                UnstructuredInformation = "Test transaction a",
                ExternalId = "TRANSACTION_CATEGORY_TEST_TRANSACTION_A",
            };
            var accountTransactionB = new InstitutionAccountTransactionEntity
            {
                AccountId = institutionAccount.Id,
                TransactionCode = "TEST_TRANSACTION",
                Amount = 20,
                Currency = "EUR",
                UnstructuredInformation = "Test transaction b",
                ExternalId = "TRANSACTION_CATEGORY_TEST_TRANSACTION_B",
            };
            var accountTransactionC = new InstitutionAccountTransactionEntity
            {
                AccountId = institutionAccount.Id,
                TransactionCode = "TEST_TRANSACTION",
                Amount = -100,
                Currency = "EUR",
                UnstructuredInformation = "Test transaction c",
                ExternalId = "TRANSACTION_CATEGORY_TEST_TRANSACTION_C",
            };

            context.Institutions.Add(institution);
            context.InstitutionConnections.Add(institutionConnection);
            context.InstitutionAccounts.Add(institutionAccount);
            context.InstitutionAccountTransactions.AddRange(accountTransactionA, accountTransactionB, accountTransactionC);
        });
    }
}
