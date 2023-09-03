using App.Backend.Controllers;
using App.Backend.Data.Entity;
using App.Backend.Test.Fixtures;

namespace App.Backend.Test.Controllers;

public class InstitutionControllerTest : GraphControllerTest<GraphQLFixture<InstitutionController>>
{
	public InstitutionControllerTest(GraphQLFixture<InstitutionController> fixture) : base(fixture)
	{
	}
	
	[Fact]
	public async Task ById_Success()
	{
		var addResult = await Fixture.Database.Institutions.AddAsync(new InstitutionEntity()
		{
			Name = "MyFakeName",
			ExternalId = "SomeExternalId"
		});
		await Fixture.Database.SaveChangesAsync();

		var result = await Fixture.ExecuteQuery(new { addResult.Entity.Id });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_NotFound()
	{
		var result = await Fixture.ExecuteQuery(new { Id = Guid.NewGuid() });
		result.MatchSnapshot();
	}
}