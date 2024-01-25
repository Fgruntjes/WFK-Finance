using System.Net;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Test;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryGetTest : IClassFixture<TransactionCategoryFixture>
{
    private readonly TransactionCategoryFixture _fixture;

    public TransactionCategoryGetTest(TransactionCategoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync<TransactionCategoryDto>(
            $"{TransactionCategoryListController.RouteBase}/{_fixture.TransactionCategoryEntity.Id}");

        // Assert
        response.Should().BeEquivalentTo(new TransactionCategoryDto()
        {
            Id = _fixture.TransactionCategoryEntity.Id,
            Name = _fixture.TransactionCategoryEntity.Name,
            Group = _fixture.TransactionCategoryEntity.Group,
            ParentId = _fixture.TransactionCategoryEntity.ParentId,
        });
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync(
            $"{TransactionCategoryListController.RouteBase}/{_fixture.TransactionCategoryEntity.Id}",
            userId: FunctionalTestFixture.AltTestUserId);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvalidId()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync(
            $"{TransactionCategoryListController.RouteBase}/ad0f0913-1e78-4073-a5ab-d6db2d076023",
            userId: FunctionalTestFixture.AltTestUserId);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
