
using App.Backend.Controllers;
using App.Backend.Dto;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryListTest : IClassFixture<TransactionCategoryFixture>
{
    private TransactionCategoryFixture _fixture;

    public TransactionCategoryListTest(TransactionCategoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        // Act
        var response = await _fixture.Client.GetWithAuthAsync<ICollection<TransactionCategoryDto>>(TransactionCategoryListController.RouteBase);

        // Assert
        response.Should().BeEquivalentTo(new List<TransactionCategoryDto>()
        {
            new ()
            {
                Id = _fixture.TransactionCategoryEntity.Id,
                Name = _fixture.TransactionCategoryEntity.Name,
                Group = _fixture.TransactionCategoryEntity.Group,
                ParentId = _fixture.TransactionCategoryEntity.ParentId,
            }
        });
    }
}
