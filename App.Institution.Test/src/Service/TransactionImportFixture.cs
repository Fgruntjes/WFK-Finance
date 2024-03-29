using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;

namespace App.Institution.Test.Service;

public class TransactionImportFixture : AppFixture
{
    public InstitutionConnectionEntity InstitutionConnectionEntity { get; }
    public InstitutionAccountEntity InstitutionAccountEntity { get; }

    public TransactionImportFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        var institutionEntity = new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        InstitutionConnectionEntity = new InstitutionConnectionEntity()
        {
            OrganisationId = OrganisationId,
            InstitutionId = institutionEntity.Id,
            ExternalId = "InstitutionConnectionExternalId",
            ConnectUrl = new Uri("https://example.com/")
        };
        InstitutionAccountEntity = new InstitutionAccountEntity()
        {
            ExternalId = "44eb1c40-17c5-4412-98a0-f26368ad3366",
            Iban = "NL48INGB0001234128",
            InstitutionConnectionId = InstitutionConnectionEntity.Id,
            ImportStatus = ImportStatus.Queued,
        };

        Database.SeedData(context =>
        {
            context.Institutions.Add(institutionEntity);
            context.InstitutionConnections.Add(InstitutionConnectionEntity);
            context.InstitutionAccounts.Add(InstitutionAccountEntity);
        });
    }
}