using Moq;
using VMelnalksnis.NordigenDotNet.Institutions;

namespace App.Backend.Test.Controllers;

public class InstitutionListTest : IClassFixture<InstitutionListFixture>
{
	private readonly InstitutionListFixture _fixture;

	public InstitutionListTest(InstitutionListFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task CountryLinkedNld()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso2 = "NL" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task CountryLinkedUsa()
	{
		var result = await _fixture.ExecuteQuery(new { CountryIso2 = "US" });
		result.MatchSnapshot();
	}

	[Fact]
	public async Task CallRefreshWhenMissing()
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