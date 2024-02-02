using App.Backend.Controllers;
using App.Lib.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionDeleteTest : IClassFixture<InstitutionConnectionFixture>
{
    private readonly InstitutionConnectionFixture _fixture;

    public InstitutionConnectionDeleteTest(InstitutionConnectionFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Delete()
    {
        // Arrange
        var institutionConnectionEntity = new InstitutionConnectionEntity
        {
            OrganisationId = _fixture.OrganisationId,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        _fixture.Database.SeedData(context =>
        {
            context.InstitutionConnections.Add(institutionConnectionEntity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{InstitutionConnectionListController.RouteBase}?id={institutionConnectionEntity.Id}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(1);
        _fixture.Database.WithData(context =>
        {
            context.InstitutionConnections.Find(institutionConnectionEntity.Id)
                .Should()
                .BeNull();
        });
    }

    [Fact]
    public async Task Delete_NotFound()
    {
        // Arrange
        var connectionId = new Guid("5dcb861b-b879-427e-ad47-4c4eade20813");

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{InstitutionConnectionListController.RouteBase}?id={connectionId}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(0);
    }

    [Fact]
    public async Task Delete_OutsideOrganisation()
    {
        // Arrange
        var institutionConnectionEntity = new InstitutionConnectionEntity
        {
            OrganisationId = _fixture.AltOrganisationId,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            ExternalId = "c513ba85-53e9-418f-97e2-0a812514aa45"
        };
        _fixture.Database.SeedData(context =>
        {
            context.InstitutionConnections.Add(institutionConnectionEntity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{InstitutionConnectionListController.RouteBase}?id={institutionConnectionEntity.Id}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(0);
        _fixture.Database.WithData(context =>
        {
            context.InstitutionConnections.Find(institutionConnectionEntity.Id)
                .Should()
                .BeEquivalentTo(institutionConnectionEntity);
        });
    }
}