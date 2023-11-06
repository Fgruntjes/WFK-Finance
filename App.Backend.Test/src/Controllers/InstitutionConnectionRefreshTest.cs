namespace App.Backend.Test.Controllers;

public class InstitutionConnectionRefreshTest : IClassFixture<InstitutionConnectionRefreshFixture>
{
	private readonly InstitutionConnectionRefreshFixture _fixture;

	public InstitutionConnectionRefreshTest(InstitutionConnectionRefreshFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task ByExternalId_Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionConnectionEntity.ExternalId
		});
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ByExternalId_MissingConnection()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			ExternalId = "SomeExternalIdMissing"
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}

	[Fact]
	public async Task ById_Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionConnectionEntity.Id
		});
		result.MatchSnapshot();

		// Assert database
		var connectionEntity = await _fixture.Database.InstitutionConnections
			.FindAsync(_fixture.InstitutionConnectionEntity.Id);
		connectionEntity.Should().NotBeNull();
		connectionEntity?.Accounts.Should().HaveCount(2);
	}

	[Fact]
	public async Task ById_MissingConnection()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			Id = new Guid("59a35c45-6e8d-4dc7-bacc-f151f94da93d")
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}
}