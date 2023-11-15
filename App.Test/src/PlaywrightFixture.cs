
using Microsoft.Playwright;
using Respawn;

namespace App.Test;

public class PlaywrightFixture : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private Respawner _respawner;

    private static string AppUrl => Environment.GetEnvironmentVariable("APP_URL")
        ?? "http://localhost:3000";
    private static string AppEnvironment => Environment.GetEnvironmentVariable("APP_ENVIRONMENT")
        ?? "dev";
    private static string DbConnectionString => Environment.GetEnvironmentVariable("APP_DB_CONNECTION_STRING")
        ?? "Server=localhost,1433; Database=development; User Id=sa; Password=myLeet123Password!; Encrypt=False";

    public PlaywrightFixture()
    {
        _respawner = Respawner.CreateAsync(DbConnectionString)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (_browserContext != null)
            await _browserContext.DisposeAsync();

        if (_browser != null)
            await _browser.DisposeAsync();
        
        _playwright?.Dispose();

        await _respawner.ResetAsync(DbConnectionString);
        GC.SuppressFinalize(this);
    }

    public async Task<IPage> WithPage()
    {
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        
        _browserContext ??= await _browser.NewContextAsync();
        
        var page = await _browserContext.NewPageAsync();
        await page.GotoAsync(AppUrl);

        await EnsureAuthentication(page);
        return page;
    }

    private static async Task EnsureAuthentication(IPage page)
    {
        var loginLocator = page.GetByText("Log in to");
        var authorizeLocator = page.GetByText("Authorize App");
        var appLocator = page.GetByText("WFK Finance");

        var loginPage = loginLocator.WaitForAsync();
        var authorizePage = authorizeLocator.WaitForAsync();
        var app = appLocator.WaitForAsync();

        var currentPage = await Task.WhenAny(loginPage, authorizePage, app);
        if (currentPage == loginPage)
        {
            await Login(page);

            authorizePage = authorizeLocator.WaitForAsync();
            app = appLocator.WaitForAsync();
            currentPage = await Task.WhenAny(authorizePage, app);
        }

        if (currentPage == authorizePage)
        {
            await AuthorizeApp(page);
        }

        await appLocator.WaitForAsync();
    }

    private static async Task AuthorizeApp(IPage page)
    {
        await page.GetByRole(AriaRole.Button, new() { Name = "Accept", Exact = true }).ClickAsync();
    }

    private static async Task Login(IPage page)
    {
        await page.GetByLabel("Email address").FillAsync($"test-{AppEnvironment}@test.com");
        await page.GetByLabel("Password").FillAsync("passpass$12$12");
        await page.GetByRole(AriaRole.Button, new() { Name = "Continue", Exact = true }).ClickAsync();
    }
}
