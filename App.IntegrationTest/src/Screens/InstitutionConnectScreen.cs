using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

class InstitutionConnectScreen
{
    private readonly IPage _page;

    public ILocator Locator => _page.GetByRole(AriaRole.Button, new() { Name = "Connect", Exact = true });

    public InstitutionConnectScreen(IPage page)
    {
        _page = page;
    }

    public async Task AssertIsOnScreen()
    {
        await Assertions.Expect(_page.GetByTestId("activeroute:/bank-accounts"))
            .ToBeVisibleAsync();
    }

    public async Task ClickMenuAsync()
    {
        await _page.ClickMenuAsync("/bank-accounts", "Bank institutions");
    }

    public async Task ClickAddAsync()
    {
        await _page.GetByText("Create").ClickAsync();
    }
}
