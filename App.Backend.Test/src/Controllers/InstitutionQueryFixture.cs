using App.Backend.Controllers;
using App.Backend.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionQueryFixture : AppFixture<InstitutionQuery>
{
    public Guid InstitutionId { get; private set; }
    public InstitutionQueryFixture()
    {
        var addResult = Database.Institutions.Add(new InstitutionEntity()
        {
            Name = "MyFakeName",
            ExternalId = "SomeExternalId"
        });

        Database.SaveChanges();

        InstitutionId = addResult.Entity.Id;
    }
}
