using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Test;
using Gridify;

namespace App.Backend.Test.Controllers;

public class InstitutionTransactionListTest : IClassFixture<InstitutionTransactionListFixture>
{
    private readonly InstitutionTransactionListFixture _fixture;

    public InstitutionTransactionListTest(InstitutionTransactionListFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionTransactionListController.RouteBase,
            userId: FunctionalTestFixture.AltTestUserId);
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>());
    }

    [Fact]
    public async Task SortInstitutionId()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionTransactionListController.RouteBase,
            new GridifyQuery()
            {
                OrderBy = "institutionId desc",
            });
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { InstitutionId = _fixture.InstitutionEntityA.Id },
            new() { InstitutionId = _fixture.InstitutionEntityB.Id },
        }, options => options
            .Including(e => e.InstitutionId));
    }

    [Fact]
    public async Task SortAccountIban()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionTransactionListController.RouteBase,
            new GridifyQuery()
            {
                OrderBy = "accountIban desc",
                PageSize = 2,
            });
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { AccountIban = _fixture.InstitutionAccountEntityA.Iban },
            new() { AccountIban = _fixture.InstitutionAccountEntityB.Iban },
        }, options => options
            .Including(e => e.AccountIban));
    }
}