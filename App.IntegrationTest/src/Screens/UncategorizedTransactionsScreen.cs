using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

class UncategorizedTransactionsScreen
{
    private readonly IPage _page;

    public ILocator Locator => _page.GetByTestId("activeroute:/bank-accounts/transactions/uncategorized");

    public UncategorizedTransactionsScreen(IPage page)
    {
        _page = page;
    }

    public async Task ClickMenuAsync()
    {
        await _page.ClickMenuAsync("/bank-accounts/transactions/uncategorized");
    }
}
