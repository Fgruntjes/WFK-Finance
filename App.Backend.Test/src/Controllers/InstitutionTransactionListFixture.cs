using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.Backend.Test.Controllers;

public class InstitutionTransactionFixture : AppFixture
{
    public InstitutionAccountEntity InstitutionAccountEntityA { get; }
    public InstitutionAccountEntity InstitutionAccountEntityB { get; }
    public InstitutionAccountTransactionEntity TransactionA { get; }
    public InstitutionAccountTransactionEntity TransactionB { get; }
    public InstitutionEntity InstitutionEntityA { get; }
    public InstitutionEntity InstitutionEntityB { get; }

    public InstitutionTransactionFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntityA = new InstitutionEntity()
        {
            ExternalId = "SomeExternalIdA",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };
        InstitutionEntityB = new InstitutionEntity()
        {
            ExternalId = "SomeExternalIdB",
            Name = "Other Institution Name",
            CountryIso2 = "NL",
        };

        var institutionConnectionEntityA = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-A",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntityA.Id,
            OrganisationId = OrganisationId,
        };
        var institutionConnectionEntityB = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-B",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntityB.Id,
            OrganisationId = OrganisationId,
        };

        var now = new LocalDateTime(2023, 12, 1, 10, 0).InUtc().ToInstant();
        InstitutionAccountEntityA = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-account-A",
            Iban = "NL00BANK0123456789",
            InstitutionConnectionId = institutionConnectionEntityA.Id,
        };
        InstitutionAccountEntityB = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-account-B",
            Iban = "NL00BANK0123450000",
            InstitutionConnectionId = institutionConnectionEntityB.Id,
        };

        TransactionA = new InstitutionAccountTransactionEntity()
        {
            ExternalId = $"account-a",
            Amount = 100,
            Currency = "EUR",
            Account = InstitutionAccountEntityA,
            UnstructuredInformation = "Some unstructured information",
            Date = now,
        };
        TransactionB = new InstitutionAccountTransactionEntity()
        {
            ExternalId = $"account-b",
            Amount = 100,
            Currency = "EUR",
            Account = InstitutionAccountEntityB,
            UnstructuredInformation = "Some unstructured information",
            Date = now,
        };

        Database.SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntityA);
            context.Institutions.Add(InstitutionEntityB);
            context.InstitutionConnections.Add(institutionConnectionEntityA);
            context.InstitutionConnections.Add(institutionConnectionEntityB);
            context.InstitutionAccounts.Add(InstitutionAccountEntityA);
            context.InstitutionAccounts.Add(InstitutionAccountEntityB);
            context.InstitutionAccountTransactions.Add(TransactionA);
            context.InstitutionAccountTransactions.Add(TransactionB);
        });
    }
}
