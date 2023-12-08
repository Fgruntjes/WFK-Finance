using App.Lib.Test.Database;
using App.Lib.Data.Entity;

namespace App.Lib.Test;

public class FunctionalTestFixture : IAsyncDisposable
{
    public PooledDatabase Database { get; }

    public readonly Guid OrganisationId;
    public readonly Guid AltOrganisationId;

    public const string TestUserId = "auth0|qm3vehnjqg885o56wa1wc006";
    public const string AltTestUserId = "auth0|pg164es5iwwqrn13qy3pc4ga";

    public FunctionalTestFixture(DatabasePool databasePool)
    {
        Database = databasePool.Get();
        Database.EnsureInitialized();

        OrganisationId = new Guid("39d0abfb-441e-4b45-a5fa-2e4554217c01");
        AltOrganisationId = new Guid("b1115aee-4e0d-4c8b-8321-9edf1df91605");
        Database.SeedData(context =>
        {
            context.Organisations.Add(new OrganisationEntity
            {
                Id = OrganisationId,
                Slug = TestUserId,
            });

            context.Organisations.Add(new OrganisationEntity
            {
                Id = AltOrganisationId,
                Slug = AltTestUserId,
            });
        });
    }

    public async ValueTask DisposeAsync()
    {
        await Database.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}