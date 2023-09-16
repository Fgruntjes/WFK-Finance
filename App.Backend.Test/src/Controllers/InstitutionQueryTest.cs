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
	public async Task Search_CountryLinkedNld()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso3 = "NLD" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task Search_CountryLinkedUsa()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso3 = "USA" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task Search_CallRefreshWhenMissing()
	{
		var institutionsMock = new Mock<IInstitutionClient>();
		institutionsMock
			.Setup(i => i.GetByCountry("GBR", It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Institution>()
			{
				new Institution{
					Id = "SomeExternalId-gbr",
					Name = "MyFakeName-gbr"
				}
			});
		_fixture.NordigenClientMoq
			.SetupGet(c => c.Institutions)
			.Returns(institutionsMock.Object);

		var result = await _fixture.ExecuteQuery(new { CountryIso3 = "GBR" });
		result.MatchSnapshot();
	}
}