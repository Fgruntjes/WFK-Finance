using App.Backend.Test.Database;
using App.Lib.Data.Entity;
using Microsoft.Extensions.Logging;

namespace App.Backend.Test.Controllers;

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

        SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
        });
    }
}