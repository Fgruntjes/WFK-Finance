using Moq;
using VMelnalksnis.NordigenDotNet.Institutions;

namespace App.Backend.Test.Controllers;

public class InstitutionQueryTest : IClassFixture<InstitutionQueryFixture>
{
	private readonly InstitutionQueryFixture _fixture;

	public InstitutionQueryTest(InstitutionQueryFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task ById_Success()
	{
		var result = await _fixture.ExecuteQuery(new { Id = _fixture.InstitutionIdNldLinked });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task ById_NotFound()
	{
		var result = await _fixture.ExecuteQuery(new { Id = Guid.NewGuid() });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task List_CountryLinkedNld()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso2 = "NL" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task List_CountryLinkedUsa()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso2 = "US" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task List_CallRefreshWhenMissing()
	{
		var institutionsMock = new Mock<IInstitutionClient>();
		institutionsMock
			.Setup(i => i.GetByCountry("GBR", It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Institution>()
			{
				new Institution{
					Id = "SomeExternalId-GB",
					Name = "MyFakeName-GB"
				}
			});
		_fixture.NordigenClientMoq
			.SetupGet(c => c.Institutions)
			.Returns(institutionsMock.Object);

		var result = await _fixture.ExecuteQuery(new { CountryIso2 = "GBR" });
		result.MatchSnapshot();
	}
}