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
    public async Task WithoutSkipLimit()
    {
        // Act
        var result = await _fixture.Client
            .GetWithAuthAsync<ListResult<InstitutionConnection>>("/institutionconnection");

        // Assert
        result.Items.Should().BeEquivalentTo(new List<InstitutionConnection>()
        {
            new() { ExternalId = "SomeExternalId-organisation-match-0" },
            new() { ExternalId = "SomeExternalId-organisation-list-0" },
            new() { ExternalId = "SomeExternalId-organisation-list-1" },
            new() { ExternalId = "SomeExternalId-organisation-list-2" },
            new() { ExternalId = "SomeExternalId-organisation-list-3" },
            new() { ExternalId = "SomeExternalId-organisation-list-4" },
            new() { ExternalId = "SomeExternalId-organisation-list-5" },
            new() { ExternalId = "SomeExternalId-organisation-list-6" },
            new() { ExternalId = "SomeExternalId-organisation-list-7" },
            new() { ExternalId = "SomeExternalId-organisation-list-8" },
            new() { ExternalId = "SomeExternalId-organisation-list-9" },
            new() { ExternalId = "SomeExternalId-organisation-list-10" },
            new() { ExternalId = "SomeExternalId-organisation-list-11" },
            new() { ExternalId = "SomeExternalId-organisation-list-12" },
            new() { ExternalId = "SomeExternalId-organisation-list-13" },
            new() { ExternalId = "SomeExternalId-organisation-list-14" },
            new() { ExternalId = "SomeExternalId-organisation-list-15" },
            new() { ExternalId = "SomeExternalId-organisation-list-16" },
            new() { ExternalId = "SomeExternalId-organisation-list-17" },
            new() { ExternalId = "SomeExternalId-organisation-list-18" },
            new() { ExternalId = "SomeExternalId-organisation-list-19" },
            new() { ExternalId = "SomeExternalId-organisation-list-20" },
            new() { ExternalId = "SomeExternalId-organisation-list-21" },
            new() { ExternalId = "SomeExternalId-organisation-list-22" },
            new() { ExternalId = "SomeExternalId-organisation-list-23" },
        },
        options => options.Excluding(e => e.Id)
            .Excluding(e => e.ConnectUrl)
            .Excluding(e => e.Accounts)
            .Excluding(e => e.InstitutionId));
        result.TotalCount.Should().Be(31);
    }

    [Fact]
    public async Task WithSkipLimit()
    {
        // Act
        var result = await _fixture.Client
            .GetWithAuthAsync<ListResult<InstitutionConnection>>("/institutionconnection?offset=1&limit=1");

        // Assert
        result.Items.Should().BeEquivalentTo(new List<InstitutionConnection>()
        {
            new() { ExternalId = "SomeExternalId-organisation-list-0" },
        },
        options => options.Excluding(e => e.Id)
            .Excluding(e => e.ConnectUrl)
            .Excluding(e => e.Accounts)
            .Excluding(e => e.InstitutionId));
        result.TotalCount.Should().Be(31);
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
        var result = await _fixture.Client
            .GetWithAuthAsync<ListResult<InstitutionConnection>>("/institutionconnection?offset=0&limit=100");

        // Assert
        result.Items
            .Should()
            .NotContain(c => c.ExternalId == "SomeExternalId-organisation-missmatch-0");
        result.TotalCount
            .Should()
            .Be(31);
    }
}