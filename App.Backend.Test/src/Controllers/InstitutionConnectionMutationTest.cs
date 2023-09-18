namespace App.Backend.Test.Controllers;

public class InstitutionConnectionMutationTest : IClassFixture<InstitutionConnectionMutationFixture>
{
	private readonly InstitutionConnectionMutationFixture _fixture;

	public InstitutionConnectionMutationTest(InstitutionConnectionMutationFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Create_Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionId,
			ReturnUrl = "http://www.example.com/return"
		});
		result.MatchSnapshot();
	}

	[Fact]
	public async Task Create_MissingInstitution()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			InstitutionId = new Guid("63ee89f0-d929-4f13-9636-4efd23aee070"),
			ReturnUrl = "http://www.example.com/return"
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}

	[Fact]
	public async Task Refresh_ByExternalId_Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionConnectionEntity.ExternalId
		});
		result.MatchSnapshot();
	}

	[Fact]
	public async Task Refresh_ByExternalId_MissingConnection()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			ExternalId = "SomeExternalIdMissing"
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}

	[Fact]
	public async Task Refresh_ById_Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionConnectionEntity.Id
		});
		result.MatchSnapshot();
	}

	[Fact]
	public async Task Refresh_ById_MissingConnection()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			Id = new Guid("59a35c45-6e8d-4dc7-bacc-f151f94da93d")
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}

	[Fact]
	public async Task Delete_Success()
	{
		throw new NotImplementedException();
	}

	[Fact]
	public async Task Delete_MissingConnection()
	{
		throw new NotImplementedException();
	}
}