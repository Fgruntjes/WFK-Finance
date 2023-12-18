using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using Moq;

namespace App.Backend.Test.Controllers;

public class InstitutionListTest : IClassFixture<AppFixture>
{
    private readonly AppFixture _fixture;

    public InstitutionListTest(AppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Search()
    {
        // Arrange
        _fixture.Services.WithMock<IInstitutionSearchService>(mock =>
        {
            mock.Setup(s => s.Search(
                "NL",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InstitutionEntity>()
                {
                    new ()
                    {
                        Name = "MyFakeName-NL1",
                        ExternalId = "SomeExternalId-NL1",
                        CountryIso2 = "NL"
                    },
                    new ()
                    {
                        Name = "MyFakeName-NL2",
                        ExternalId = "SomeExternalId-NL2",
                        CountryIso2 = "NL"
                    }
                });
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new { CountryIso2 = "NL" });

        // Assert
        result.MatchSnapshot();
    }
}