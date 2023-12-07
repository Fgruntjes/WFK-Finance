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
        var result = await _fixture.Client.ExecuteQuery(new { _fixture.InstitutionConnectionEntity.Id });

        // Assert
        result.MatchSnapshot();
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
        _fixture.SeedData(c =>
        {
            c.InstitutionConnections.Add(connectionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new { connectionEntity.Id });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task WithInstitution()
    {
        // Act
        var result = await _fixture.Client.ExecuteQuery(new { _fixture.InstitutionConnectionEntity.Id });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task WithAccounts()
    {
        // Arrange
        var InstitutionConnectionAccounts = new List<InstitutionConnectionAccountEntity>();
        for (int i = 0; i < 3; i++)
        {
            InstitutionConnectionAccounts.Add(new InstitutionConnectionAccountEntity()
            {
                ExternalId = $"SomeExternalId-organisation-account-{i}",
                InstitutionConnectionId = _fixture.InstitutionConnectionEntity.Id,
                Iban = $"IBAN{i}",
            });
        }
        _fixture.SeedData(c =>
        {
            c.InstitutionConnectionAccounts.AddRange(InstitutionConnectionAccounts);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new { Id = _fixture.InstitutionConnectionEntity.Id });

        // Assert
        result.MatchSnapshot();
    }
}