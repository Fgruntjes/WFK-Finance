using App.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionDeleteTest : IClassFixture<InstitutionConnectionFixture>
{
    private readonly InstitutionConnectionFixture _fixture;

    public InstitutionConnectionDeleteTest(InstitutionConnectionFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var institutionConnectionEntity = new InstitutionConnectionEntity
        {
            OrganisationId = _fixture.OrganisationId,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        _fixture.SeedData(context =>
        {
            context.InstitutionConnections.Add(institutionConnectionEntity);
        });

        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            ConnectionIds = new List<Guid> { institutionConnectionEntity.Id }
        });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task MissingConnection()
    {
        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            ConnectionIds = new List<Guid> { new("5dcb861b-b879-427e-ad47-4c4eade20813") }
        });

        // Assert
        result.MatchSnapshot();
    }
}