namespace App.Backend.Test.Controllers;

public class InstitutionConnectionGetTest : IClassFixture<InstitutionConnectionGetFixture>
{
	private readonly InstitutionConnectionGetFixture _fixture;

	public InstitutionConnectionGetTest(InstitutionConnectionGetFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Success()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.OrganisationMatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task OrganisationMismatch()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.OrganisationMissmatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task WithInstitution()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.OrganisationMatchConnection });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task WithAccounts()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.OrganisationMatchConnection });
		result.MatchSnapshot();
	}
}