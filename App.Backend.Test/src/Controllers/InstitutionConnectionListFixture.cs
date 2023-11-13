using App.Backend.Test.Database;
using App.Data.Entity;
using Microsoft.Extensions.Logging;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionListFixture : InstitutionConnectionFixture
{
    public InstitutionConnectionListFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        var connectionList = new List<InstitutionConnectionEntity>();
        for (int i = 0; i < 30; i++)
        {
            connectionList.Add(new InstitutionConnectionEntity()
            {
                ExternalId = $"SomeExternalId-organisation-list-{i}",
                ConnectUrl = new Uri($"https://www.example-organisation-list-{i}.com/"),
                InstitutionId = InstitutionEntity.Id,
                OrganisationId = OrganisationId,
            });
        }

        SeedData(context =>
        {
            context.InstitutionConnections.AddRange(connectionList);
        });
    }
}
