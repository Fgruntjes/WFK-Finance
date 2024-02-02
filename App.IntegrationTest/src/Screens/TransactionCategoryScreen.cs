using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

class TransactionCategoryScreen
{
    private readonly IPage _page;

    public ILocator Locator => _page.GetByTestId("activeroute:/transaction-categories");

    public TransactionCategoryScreen(IPage page)
    {
        _page = page;
    }

    public async Task ClickMenuAsync()
    {
        await _page.ClickMenuAsync("/transaction-categories");
    }
}
