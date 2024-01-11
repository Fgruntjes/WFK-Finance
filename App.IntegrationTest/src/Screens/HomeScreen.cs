using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

class HomeScreen
{
    private readonly IPage _page;

    public ILocator Locator => _page.GetByTestId("activeroute:/");

    public ILocator UserInfoLocator => _page.GetByText("test-dev@test.com");

    public HomeScreen(IPage page)
    {
        _page = page;
    }
}
