using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using App.TransactionCategory.Exception;
using App.TransactionCategory.Interface;
using App.TransactionCategory.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.TransactionCategory.Test.Service;

public class TransctionCategoryCreateServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public TransctionCategoryCreateServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void ImplementationType()
    {
        // Act
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);

        // Act
        var bus = fixture.Services.GetRequiredService<ITransactionCategoryService>();

        // Assert
        bus.Should().BeOfType<TransactionCategoryService>();
    }

    [Fact]
    public async Task CreateWithinOrganisation()
    {
        // Arrange
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);

        // Act
        var result = await fixture.Services.GetRequiredService<ITransactionCategoryService>()
            .CreateAsync("Test", CategoryGroup.Expense, null, default);

        // Assert
        result.Should().BeEquivalentTo(new TransactionCategoryEntity()
        {
            Name = "Test",
            Group = CategoryGroup.Expense,
            OrganisationId = fixture.OrganisationId,
        }, o => o.Excluding(e => e.Id));
    }

    [Fact]
    public async Task MissingParent()
    {
        // Arrange
        var parentId = new Guid("3b43cf63-8392-4726-a50b-3d78ff3d5537");
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionCategoryService>();

        // Act
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
           service.CreateAsync("Test", CategoryGroup.Expense, parentId, default));

        // Assert
        exception.Should().BeEquivalentTo(new CategoryNotFoundException(parentId));
    }

    [Fact]
    public async Task ParentWithinOrganisation()
    {
        // Arrange
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);
        var parentEntity = new TransactionCategoryEntity()
        {
            Name = "Test parent",
            Group = CategoryGroup.Other,
            ParentId = null,
            OrganisationId = fixture.AltOrganisationId,
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(parentEntity);
        });
        var service = fixture.Services.GetRequiredService<ITransactionCategoryService>();

        // Act
        var exception = await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
           service.CreateAsync("Test", CategoryGroup.Expense, parentEntity.Id, default));

        // Assert
        exception.Should().BeEquivalentTo(new CategoryNotFoundException(parentEntity.Id));
    }
}