using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.Backend.Test.Controllers;

public class InstitutionAccountTransactionListFixture : AppFixture
{
    public Instant Now { get; }
    public InstitutionAccountEntity InstitutionAccountEntity { get; }
    public InstitutionEntity InstitutionEntity { get; }

    public InstitutionAccountTransactionListFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntity = new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        var institutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-0",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntity.Id,
            OrganisationId = OrganisationId,
        };

        Now = new LocalDateTime(2023, 12, 1, 10, 0).InUtc().ToInstant();
        InstitutionAccountEntity = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-0",
            Iban = "NL00BANK0123456789",
            InstitutionConnectionId = institutionConnectionEntity.Id,
        };

        var transactionDate = Now;
        var transactionList = new List<InstitutionAccountTransactionEntity>();
        for (var i = 0; i < 30; i++)
        {
            transactionList.Add(new InstitutionAccountTransactionEntity()
            {
                ExternalId = $"SomeExternalId-organisation-match-{i}",
                Amount = 100,
                Currency = "EUR",
                Account = InstitutionAccountEntity,
                UnstructuredInformation = "Some unstructured information",
                Date = transactionDate,
            });
            transactionDate = transactionDate.Plus(Duration.FromHours(1));
        }

        Database.SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
            context.InstitutionConnections.Add(institutionConnectionEntity);
            context.InstitutionAccounts.Add(InstitutionAccountEntity);
            context.InstitutionAccountTransactions.AddRange(transactionList);
        });
    }
}