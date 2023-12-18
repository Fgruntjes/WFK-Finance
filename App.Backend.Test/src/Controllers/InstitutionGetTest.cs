using App.Lib.Data.Entity;

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

        _fixture.Database.SeedData(context =>
        {
            context.Institutions.Add(institutionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new { institutionEntity.Id });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task NotFound()
    {
        // Act
        var result = await _fixture.Client.ExecuteQuery(new { Id = new Guid("484cc24c-3a50-4b05-b550-c7c1be8eed05") });

        // Assert
        result.MatchSnapshot();
    }
}