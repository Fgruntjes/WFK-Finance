using App.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionGetTest : IClassFixture<AppFixture>
{
    private readonly AppFixture _fixture;

    public InstitutionGetTest(AppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var institutionEntity = new InstitutionEntity()
        {
            Name = "MyFakeName-NL",
            ExternalId = "SomeExternalId-NL",
            CountryIso2 = "NL"
        };

        _fixture.SeedData(context =>
        {
            context.Institutions.Add(institutionEntity);
        });

        // Act
        var result = await _fixture.ExecuteQuery(new { Id = institutionEntity.Id });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task NotFound()
    {
        // Act
        var result = await _fixture.ExecuteQuery(new { Id = Guid.NewGuid() });

        // Assert
        result.MatchSnapshot();
    }
}