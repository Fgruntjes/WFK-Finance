namespace App.Backend.Test.Controllers;

public class InstitutionGetTest : IClassFixture<InstitutionGetFixture>
{
	private readonly InstitutionGetFixture _fixture;

	public InstitutionGetTest(InstitutionGetFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Success()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.InstitutionId });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task NotFound()
	{
		var result = await _fixture.ExecuteQuery(new { Id = Guid.NewGuid() });
		result.MatchSnapshot();
	}
}