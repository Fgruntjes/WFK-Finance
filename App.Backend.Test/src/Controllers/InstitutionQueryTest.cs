namespace App.Backend.Test.Controllers;

public class InstitutionQueryTest
{
	private readonly InstitutionQueryFixture _fixture;

	public InstitutionQueryTest(InstitutionQueryFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task ById_Success()
	{
		var result = await _fixture.ExecuteQuery(new { _fixture.InstitutionId });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_NotFound()
	{
		var result = await _fixture.ExecuteQuery(new { Id = Guid.NewGuid() });
		result.MatchSnapshot();
	}
}