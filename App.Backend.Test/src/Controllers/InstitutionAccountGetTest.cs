using System.Net;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionAccountGetTest : IClassFixture<InstitutionAccountFixture>
{
    private readonly InstitutionAccountFixture _fixture;

    public InstitutionAccountGetTest(InstitutionAccountFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync<InstitutionAccountDto>(
            $"{InstitutionAccountListController.RouteBase}/{_fixture.InstitutionAccountEntity.Id}");

        // Assert
        response.Should().BeEquivalentTo(new InstitutionAccountDto()
        {
            ExternalId = _fixture.InstitutionAccountEntity.ExternalId,
            InstitutionId = _fixture.InstitutionAccountEntity.InstitutionConnection.InstitutionId,
            Id = _fixture.InstitutionAccountEntity.Id,
            Iban = _fixture.InstitutionAccountEntity.Iban,
        });
    }

    [Fact]
    public async Task OrganisationMismatch()
    {
        // Arrange
        var connectionEntity = new InstitutionConnectionEntity()
        {
            InstitutionId = _fixture.InstitutionEntity.Id,
            ExternalId = $"SomeExternalId-organisation-missmatch-0",
            OrganisationId = _fixture.AltOrganisationId,
            ConnectUrl = new Uri($"https://www.example-organisation-missmatch-0.com/"),
        };
        var accountEntity = new InstitutionAccountEntity()
        {
            ExternalId = $"SomeExternalId-organisation-missmatch-0",
            InstitutionConnectionId = connectionEntity.Id,
            Iban = "NL00BANK0123456789",
        };
        _fixture.Database.SeedData(c =>
        {
            c.InstitutionConnections.Add(connectionEntity);
            c.InstitutionAccounts.Add(accountEntity);
        });

        // Act
        var response = await _fixture.Client.GetWithAuthAsync(
            $"/institutionconnection/${connectionEntity.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
