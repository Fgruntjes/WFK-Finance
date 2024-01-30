using App.Backend.Controllers;
using App.Lib.Data.Entity;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryDeleteTest : IClassFixture<TransactionCategoryFixture>
{
    private readonly TransactionCategoryFixture _fixture;

    public TransactionCategoryDeleteTest(TransactionCategoryFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task Delete()
    {
        // Arrange
        var entity = new TransactionCategoryEntity
        {
            OrganisationId = _fixture.OrganisationId,
            Group = TransactionCategoryGroup.Expense,
            Name = "Test",
        };
        _fixture.Database.SeedData(context =>
        {
            context.TransactionCategory.Add(entity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{TransactionCategoryListController.RouteBase}?id={entity.Id}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(1);
        _fixture.Database.WithData(context =>
        {
            context.InstitutionConnections.Find(entity.Id)
                .Should()
                .BeNull();
        });
    }

    [Fact]
    public async Task Delete_NotFound()
    {
        // Arrange
        var id = new Guid("5dcb861b-b879-427e-ad47-4c4eade20813");

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{TransactionCategoryListController.RouteBase}?id={id}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(0);
    }

    [Fact]
    public async Task Delete_OutsideOrganisation()
    {
        // Arrange
        var entity = new TransactionCategoryEntity
        {
            OrganisationId = _fixture.OrganisationId,
            Group = TransactionCategoryGroup.Expense,
            Name = "Test",
        };
        _fixture.Database.SeedData(context =>
        {
            context.TransactionCategory.Add(entity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{InstitutionConnectionListController.RouteBase}?id={entity.Id}"));
        var body = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        body.Should().Be(0);
        _fixture.Database.WithData(context =>
        {
            context.TransactionCategory.Find(entity.Id)
                .Should()
                .BeEquivalentTo(entity);
        });
    }
}