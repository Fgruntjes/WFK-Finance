using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;

namespace App.Lib.InstitutionConnection.Test.Service;

public class InstitutionConnectionCreateFixture : AppFixture
{
    public InstitutionEntity InstitutionEntity { get; }

    public InstitutionConnectionCreateFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntity = new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        Database.SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
        });
    }
}