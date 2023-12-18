using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

class Auth0Screen
{
    private readonly IPage _page;

    public ILocator AuthorizeLocator => _page.GetByText("Authorize App");
    public ILocator LoginLocator => _page.GetByText("Log in to");

    public Auth0Screen(IPage page)
    {
        _page = page;
    }

    public async Task AcceptAuthorization()
    {
        await _page
            .GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = true })
            .ClickAsync();
    }

    public async Task Login(string email, string password)
    {
        await _page.GetByLabel("Email address").FillAsync(email);
        await _page.GetByLabel("Password").FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Continue", Exact = true }).ClickAsync();
    }
}
