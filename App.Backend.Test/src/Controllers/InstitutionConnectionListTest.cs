using App.Lib.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionListTest : IClassFixture<InstitutionConnectionListFixture>
{
    private readonly InstitutionConnectionListFixture _fixture;

    public InstitutionConnectionListTest(InstitutionConnectionListFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WithoutSkipLimit()
    {
        // Act
        var result = await _fixture.Client.ExecuteQuery();

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task WithSkipLimit()
    {
        // Act
        var result = await _fixture.Client.ExecuteQuery(new { offset = 1, limit = 1 });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        // Arrange
        var connectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = $"SomeExternalId-organisation-missmatch-0",
            ConnectUrl = new Uri($"https://www.example-organisation-missmatch-0.com/"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            OrganisationId = _fixture.AltOrganisationId,
        };
        _fixture.Database.SeedData(c =>
        {
            c.InstitutionConnections.Add(connectionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery();

        // Assert
        result.MatchSnapshot();
    }
}