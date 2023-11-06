namespace App.Backend.Test.Controllers;

public class InstitutionConnectionDeleteTest : IClassFixture<InstitutionConnectionDeleteFixture>
{
	private readonly InstitutionConnectionDeleteFixture _fixture;

	public InstitutionConnectionDeleteTest(InstitutionConnectionDeleteFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			ConnectionIds = new List<Guid> { _fixture.InstitutionConnectionId }
		});
		result.MatchSnapshot();
	}

	[Fact]
	public async Task MissingConnection()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			ConnectionIds = new List<Guid> { new("5dcb861b-b879-427e-ad47-4c4eade20813") }
		});
		result.MatchSnapshot();
	}
}