using App.Backend.Controllers;
using App.Backend.Data.Entity;
using App.Backend.Test.Fixtures;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionControllerTest : GraphControllerTest<GraphQLFixture<InstitutionConnectionController>>
{
	public InstitutionConnectionControllerTest(GraphQLFixture<InstitutionConnectionController> fixture) : base(fixture)
	{}
	
	[Fact]
	public async Task ById_WithoutSubTypes()
	{
		var addResult = await Fixture.Database.InstitutionConnections.AddAsync(new InstitutionConnectionEntity()
		{
			ExternalId = "SomeExternalId",
			ConnectUrl = "SomeConnectUrl"
		});
		await Fixture.Database.SaveChangesAsync();
		
		var result = await Fixture.ExecuteQuery(new { addResult.Entity.Id });
		result.MatchSnapshot();
	}
	
	[Fact]
	public async Task ById_WithInstitution()
	{
		var institutionsAddResult = await Fixture.Database.Institutions.AddAsync(new InstitutionEntity()
		{
			ExternalId = "SomeExternalId",
			Name = "SomeName"
		});
		
		var addResult = await Fixture.Database.InstitutionConnections.AddAsync(new InstitutionConnectionEntity()
		{
			ExternalId = "SomeExternalId",
			ConnectUrl = "SomeConnectUrl",
			InstitutionId = institutionsAddResult.Entity.Id
		});
		await Fixture.Database.SaveChangesAsync();
		
		var result = await Fixture.ExecuteQuery(new { addResult.Entity.Id });
		result.MatchSnapshot();
	}
	
	[Fact]
	public async Task ById_WithOrganisation()
	{
		var organisationAddResult = await Fixture.Database.Organisations.AddAsync(new OrganisationEntity()
		{
			Slug = "some-slug"
		});
		
		var addResult = await Fixture.Database.InstitutionConnections.AddAsync(new InstitutionConnectionEntity()
		{
			ExternalId = "SomeExternalId",
			ConnectUrl = "SomeConnectUrl",
			OrganisationId = organisationAddResult.Entity.Id
		});
		await Fixture.Database.SaveChangesAsync();
		
		var result = await Fixture.ExecuteQuery(new { addResult.Entity.Id });
		result.MatchSnapshot();
	}
}