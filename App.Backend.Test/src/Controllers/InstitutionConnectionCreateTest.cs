using Microsoft.EntityFrameworkCore;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionCreateTest : IClassFixture<InstitutionConnectionCreateFixture>
{
	private readonly InstitutionConnectionCreateFixture _fixture;

	public InstitutionConnectionCreateTest(InstitutionConnectionCreateFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Success()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			_fixture.InstitutionId,
			ReturnUrl = "http://www.example.com/return"
		});
		result.MatchSnapshot();

		// Assert database
		var connectionEntity = await _fixture.Database.InstitutionConnections
			.Where(e => e.ExternalId == _fixture.NordigenRequisitionResult.Id.ToString())
			.FirstAsync();
		connectionEntity.Should().NotBeNull();
	}

	[Fact]
	public async Task MissingInstitution()
	{
		var result = await _fixture.ExecuteQuery(new
		{
			InstitutionId = new Guid("63ee89f0-d929-4f13-9636-4efd23aee070"),
			ReturnUrl = "http://www.example.com/return"
		});
		result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
	}
}