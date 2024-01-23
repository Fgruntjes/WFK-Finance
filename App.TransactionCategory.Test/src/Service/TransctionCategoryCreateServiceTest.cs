using App.Lib.Data;
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
    public async Task Create()
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
        }, o => o.Excluding(e => e.Id).Excluding(e => e.CreatedAt));
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);
        var parent = new TransactionCategoryEntity()
        {
            Id = new Guid("32e89720-31ad-49f5-8a87-f210403f456e"),
            Name = "Parent",
            Group = CategoryGroup.Expense,
            OrganisationId = fixture.OrganisationId,
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(parent);
        });

        // Act
        var result = await fixture.Services.GetRequiredService<ITransactionCategoryService>()
            .UpdateAsync(fixture.TransactionCategoryEntity.Id, "Test", CategoryGroup.Investment, parent.Id, default);

        // Assert
        result.Should().BeEquivalentTo(new TransactionCategoryEntity()
        {
            Name = "Test",
            Group = CategoryGroup.Investment,
            OrganisationId = fixture.OrganisationId,
            ParentId = parent.Id,
        }, o => o.Excluding(e => e.Id).Excluding(e => e.CreatedAt));
    }

    [Fact]
    public async Task Create_MissingParent()
    {
        // Arrange
        var parentId = new Guid("3b43cf63-8392-4726-a50b-3d78ff3d5537");
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionCategoryService>();

        // Act
        var act = () => service.CreateAsync(
            "Test",
            CategoryGroup.Expense,
            parentId,
            default);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .Where(e => e.Data.MustGet("Id").ToString() == parentId.ToString());
    }

    [Fact]
    public async Task Update_MissingParent()
    {
        // Arrange
        var parentId = new Guid("3b43cf63-8392-4726-a50b-3d78ff3d5537");
        var fixture = new TransactionCategoryFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionCategoryService>();

        // Act
        var act = () => service.UpdateAsync(
            fixture.TransactionCategoryEntity.Id,
            "Test",
            CategoryGroup.Expense,
            parentId,
            default);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .Where(e => e.Data.MustGet("Id").ToString() == parentId.ToString());
    }

    [Fact]
    public async Task Create_ParentOutsideOfOrganisation()
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
        var act = () => service.CreateAsync(
            "Test",
            CategoryGroup.Expense,
            parentEntity.Id,
            default);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .Where(e => e.Data.MustGet("Id").ToString() == parentEntity.Id.ToString());
    }

    [Fact]
    public async Task Update_ParentOutsideOfOrganisation()
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
        var act = () => service.UpdateAsync(
            fixture.TransactionCategoryEntity.Id,
            "Test",
            CategoryGroup.Expense,
            parentEntity.Id,
            default);

        // Assert
        await act.Should().ThrowAsync<CategoryNotFoundException>()
            .Where(e => e.Data.MustGet("Id").ToString() == parentEntity.Id.ToString());
    }
}