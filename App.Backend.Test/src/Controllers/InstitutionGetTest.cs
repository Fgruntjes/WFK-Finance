using System.Net;
using App.Backend.Controllers;
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

        //Act
        var response = await _fixture.Client.GetWithAuthAsync<Dto.Institution>($"{InstitutionListController.RouteBase}/{institutionEntity.Id}");

        // Assert
        response.Should().BeEquivalentTo(new Dto.Institution
        {
            Id = institutionEntity.Id,
            ExternalId = institutionEntity.ExternalId,
            Name = institutionEntity.Name,
            Logo = institutionEntity.Logo,
            CountryIso2 = institutionEntity.CountryIso2,
        });
    }

    [Fact]
    public async Task NotFound()
    {
        //Act
        var response = await _fixture.Client.GetWithAuthAsync($"{InstitutionListController.RouteBase}/484cc24c-3a50-4b05-b550-c7c1be8eed05");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}