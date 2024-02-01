using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;
using Gridify;

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
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnectionDto>>();

        // Assert
        result.Should().BeEquivalentTo(new List<InstitutionConnectionDto>()
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

    private static InstitutionConnectionDto CreateConnection(string externalId)
    {
        return new InstitutionConnectionDto
        {
            ExternalId = externalId,
            Accounts = new List<InstitutionAccountDto>(),
            ConnectUrl = new Uri($"https://www.{externalId}.com/"),
        };
    }

    [Fact]
    public async Task WithRange()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionConnectionListController.RouteBase,
            query: new GridifyQuery()
            {
                Page = 2,
                PageSize = 1,
            });
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnectionDto>>();

        // Assert
        result.Should().BeEquivalentTo(new List<InstitutionConnectionDto>()
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
            query: new GridifyQuery()
            {
                Page = 1,
                PageSize = 100,
            });
        var result = await response.Content.ReadFromJsonAsync<ICollection<InstitutionConnectionDto>>();

        // Assert
        result.Should().NotContain(c => c.ExternalId == "SomeExternalId-organisation-missmatch-0");
        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionconnections 0-31/31");
    }
}