using System.Net;
using App.Backend.Dto;
using App.Lib.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionGetTest : IClassFixture<InstitutionConnectionFixture>
{
    private readonly InstitutionConnectionFixture _fixture;

    public InstitutionConnectionGetTest(InstitutionConnectionFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync<InstitutionConnection>(
            $"/institutionconnection/{_fixture.InstitutionConnectionEntity.Id}");

        // Assert
        response.Should().BeEquivalentTo(new InstitutionConnection()
        {
            ExternalId = _fixture.InstitutionConnectionEntity.ExternalId,
            Id = _fixture.InstitutionConnectionEntity.Id,
            ConnectUrl = _fixture.InstitutionConnectionEntity.ConnectUrl,
            InstitutionId = _fixture.InstitutionConnectionEntity.InstitutionId,
            Accounts = _fixture.InstitutionConnectionEntity.Accounts.Select(a => new InstitutionConnectionAccount()
            {
                Id = a.Id,
                ExternalId = a.ExternalId,
                Iban = a.Iban,
            }).ToList(),
        });
    }

    [Fact]
    public async Task OrganisationMismatch()
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
        var response = await _fixture.Client.GetWithAuthAsync(
            $"/institutionconnection/${connectionEntity.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}