namespace App.Backend.Test.Controllers;

public class InstitutionConnectionQueryTest : IClassFixture<InstitutionConnectionQueryFixture>
{
	private readonly InstitutionConnectionQueryFixture _fixture;

	public InstitutionConnectionQueryTest(InstitutionConnectionQueryFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task ById_WithoutSubTypes()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.FirstOrganisationMatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_WithInstitution()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.FirstOrganisationMatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_WithAccounts()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.FirstOrganisationMatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_OrganisationMismatch()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.FirstOrganisationMissmatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task List_WithoutSkipLimit()
	{
		var result = await _fixture.ExecuteQuery();
		result.MatchSnapshot();
	}

	[Fact]
	public async Task List_WithSkipLimit()
	{
		var result = await _fixture.ExecuteQuery(new { offset = 1, limit = 1 });
		result.MatchSnapshot();
	}
}