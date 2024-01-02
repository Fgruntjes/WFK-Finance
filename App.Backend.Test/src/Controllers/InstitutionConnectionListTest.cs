using System.Net.Http.Json;
using App.Backend.Controllers;
using App.Backend.Dto;
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
    public async Task WithoutRange()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(InstitutionConnectionListController.RouteBase);
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnection>>();

        // Assert
        result.Should().BeEquivalentTo(new List<InstitutionConnection>()
        {
            CreateConnection("SomeExternalId-organisation-match-0"),
            CreateConnection("SomeExternalId-organisation-list-0"),
            CreateConnection("SomeExternalId-organisation-list-1"),
            CreateConnection("SomeExternalId-organisation-list-2"),
            CreateConnection("SomeExternalId-organisation-list-3"),
            CreateConnection("SomeExternalId-organisation-list-4"),
            CreateConnection("SomeExternalId-organisation-list-5"),
            CreateConnection("SomeExternalId-organisation-list-6"),
            CreateConnection("SomeExternalId-organisation-list-7"),
            CreateConnection("SomeExternalId-organisation-list-8"),
            CreateConnection("SomeExternalId-organisation-list-9"),
            CreateConnection("SomeExternalId-organisation-list-10"),
            CreateConnection("SomeExternalId-organisation-list-11"),
            CreateConnection("SomeExternalId-organisation-list-12"),
            CreateConnection("SomeExternalId-organisation-list-13"),
            CreateConnection("SomeExternalId-organisation-list-14"),
            CreateConnection("SomeExternalId-organisation-list-15"),
            CreateConnection("SomeExternalId-organisation-list-16"),
            CreateConnection("SomeExternalId-organisation-list-17"),
            CreateConnection("SomeExternalId-organisation-list-18"),
            CreateConnection("SomeExternalId-organisation-list-19"),
            CreateConnection("SomeExternalId-organisation-list-20"),
            CreateConnection("SomeExternalId-organisation-list-21"),
            CreateConnection("SomeExternalId-organisation-list-22"),
            CreateConnection("SomeExternalId-organisation-list-23"),
        },
        options => options.Excluding(e => e.Id)
            .Excluding(e => e.ConnectUrl)
            .Excluding(e => e.Accounts)
            .Excluding(e => e.InstitutionId));
        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionconnections 0-25/31");
    }

    private static InstitutionConnection CreateConnection(string externalId)
    {
        return new InstitutionConnection
        {
            ExternalId = externalId,
            Accounts = new List<InstitutionConnectionAccount>(),
            ConnectUrl = new Uri($"https://www.{externalId}.com/"),
        };
    }

    [Fact]
    public async Task WithRange()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionConnectionListController.RouteBase,
            range: new RangeParameter(1, 2));
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnection>>();

        // Assert
        result.Should().BeEquivalentTo(new List<InstitutionConnection>()
        {
            CreateConnection("SomeExternalId-organisation-list-0"),
        },
        options => options.Excluding(e => e.Id)
            .Excluding(e => e.ConnectUrl)
            .Excluding(e => e.Accounts)
            .Excluding(e => e.InstitutionId));
        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionconnections 1-2/31");
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        // Arrange
        var connectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-missmatch-0",
            ConnectUrl = new Uri($"https://www.example-organisation-missmatch-0.com/"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            OrganisationId = _fixture.AltOrganisationId,
        };
        _fixture.Database.SeedData(c =>
        {
            c.InstitutionConnections.Add(connectionEntity);
        });

        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionConnectionListController.RouteBase,
            range: new RangeParameter(0, 100));
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnection>>();

        // Assert
        result.Should().NotContain(c => c.ExternalId == "SomeExternalId-organisation-missmatch-0");
        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionconnections 0-31/31");
    }
}