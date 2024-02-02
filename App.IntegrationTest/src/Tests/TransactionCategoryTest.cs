using App.IntegrationTest.Screens;
using Microsoft.Playwright;

namespace App.IntegrationTest.Tests;

public class TransactionCategoryTest : IClassFixture<TransactionCategoryFixture<InstitutionConnectTest>>
{
    private readonly TransactionCategoryFixture<InstitutionConnectTest> _fixture;

    public TransactionCategoryTest(TransactionCategoryFixture<InstitutionConnectTest> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CategorizeTransactions()
    {
        var page = await _fixture.WithPage();
        var transactionScreen = new AccountTransactionScreen(page);
        var categoryScreen = new TransactionCategoryScreen(page);
        var uncategorizedTransactionsScreen = new UncategorizedTransactionsScreen(page);

        // Create categories
        await categoryScreen.ClickMenuAsync();
        await page.GetByText("Create defaults").ClickAsync();
        await page.GetByText("Guilt-free spending").IsVisibleAsync();

        // Categorize transactions
        await uncategorizedTransactionsScreen.ClickMenuAsync();
        await page.SearchSelectOptionAsync("transaction-category-0", "Transportation");
        await page.SearchSelectOptionAsync("transaction-category-1", "Groceries");
        await page.SearchSelectOptionAsync("transaction-category-2", "Salary");
        await Assertions.Expect(page.Locator(".ant-select-loading")).ToHaveCountAsync(0);

        // Go back to transaction view and check if transactions are categorized
        await transactionScreen.ClickMenuAsync();
        await page.GetByText("Transportation").IsVisibleAsync();
        await page.GetByText("Groceries").IsVisibleAsync();
        await page.GetByText("Salary").IsVisibleAsync();
    }
}