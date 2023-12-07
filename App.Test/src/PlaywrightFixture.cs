using App.Test.Screens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Respawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using App.Lib.Configuration.Options;
using Microsoft.Extensions.Logging;

namespace App.Test;

public class PlaywrightFixture : IAsyncLifetime
{
    public IServiceProvider Services { get; }

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private readonly Respawner _respawner;
    private readonly string _dbConnectionString;

    public PlaywrightFixture(ILoggerProvider loggerProvider)
    {
        var applicationFactory = new ApplicationFactory(loggerProvider);

        Services = applicationFactory.Services;
        _dbConnectionString = Services.GetRequiredService<IConfiguration>().GetConnectionString("Database")
                              ?? throw new Exception("Database connection string not found");
        _respawner = Respawner.CreateAsync(_dbConnectionString)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task InitializeAsync()
    {
        // Ensure we start with a clean database
        await _respawner.ResetAsync(_dbConnectionString);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_browserContext != null)
            await _browserContext.DisposeAsync();

        if (_browser != null)
            await _browser.DisposeAsync();

        _playwright?.Dispose();

        await _respawner.ResetAsync(_dbConnectionString);
    }

    public async Task<IPage> WithPage()
    {
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
        });

        _browserContext ??= await _browser.NewContextAsync();

        var page = await _browserContext.NewPageAsync();
        await page.GotoAsync(Services.GetRequiredService<IOptions<AppOptions>>().Value.FrontendUrl);

        await EnsureAuthentication(page);
        return page;
    }

    private async Task EnsureAuthentication(IPage page)
    {
        var appEnvironment = Services.GetRequiredService<IOptions<AppOptions>>().Value.Environment;

        var authScreen = new Auth0Screen(page);
        var homeScreen = new HomeScreen(page);

        await new List<LocatorAction> {
            new (authScreen.LoginLocator, async (_) => await authScreen.Login($"test-{appEnvironment}@test.com", "passpass$12$12")),
            new (authScreen.AuthorizeLocator, async (_) => await authScreen.AcceptAuthorization()),
            new (homeScreen.Locator)}
            .GoToLastAsync();
    }
}
