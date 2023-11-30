using App.Data.Entity;
using Moq;
using VMelnalksnis.NordigenDotNet.Institutions;

namespace App.Backend.Test.Controllers;

public class InstitutionListTest : IClassFixture<AppFixture>
{
    private readonly AppFixture _fixture;

    public InstitutionListTest(AppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CountryLinked()
    {
        // Arrange
        var institutionNldLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-NL",
            ExternalId = "SomeExternalId-NL",
            CountryIso2 = "NL"
        };
        var institutionUsaLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-US",
            ExternalId = "SomeExternalId-US",
            CountryIso2 = "US"
        };
        _fixture.SeedData(context =>
        {
            context.Institutions.Add(institutionNldLinked);
            context.Institutions.Add(institutionUsaLinked);
        });

        // Act
        var result = await _fixture.Server.ExecuteQuery(new { CountryIso2 = "NL" });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task CallRefreshWhenMissing()
    {
        // Arrange
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

        // Act
        var result = await _fixture.Server.ExecuteQuery(new { CountryIso2 = "GBR" });

        // Arrange
        result.MatchSnapshot();
    }
}