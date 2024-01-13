using App.IntegrationTest.Screens;
using App.Lib.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using App.Lib.Configuration.Options;
using App.Lib.Data;
using App.Lib.InstitutionConnection;
using App.Lib.Test.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;

namespace App.IntegrationTest;

public class AppFixture : IAsyncDisposable
{
    public Database<DatabaseContext> Database { get; }
    public IServiceProvider Services { get; }

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;

    public AppFixture(ILoggerProvider loggerProvider)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseConfiguration()
            .UseDatabase()
            .UseInstitutionConnectionClient()
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.Services.AddSingleton(loggerProvider);
            });
        var host = hostBuilder.Build();

        Services = host.Services;
        var connectionString = Services.GetRequiredService<IConfiguration>().GetConnectionString("Database")
                               ?? throw new Exception("Database connection string not found");

        Database = new Database(connectionString);
    }

    public async ValueTask DisposeAsync()
    {
        if (_browserContext != null)
            await _browserContext.DisposeAsync();

        if (_browser != null)
            await _browser.DisposeAsync();

        _playwright?.Dispose();
    }

    public async Task<IPage> WithPage()
    {
        var isCiCd = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = isCiCd
        });

        _browserContext ??= await _browser.NewContextAsync(new BrowserNewContextOptions()
        {
            RecordVideoDir = isCiCd ? GetVideoFolder() : null,
        });

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

    private static string GetVideoFolder()
    {
        var directory = Directory.GetCurrentDirectory();

        while (directory != null)
        {
            var appSettingsPath = Path.Combine(directory, "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                return Path.Combine(directory, ".test-videos");
            }
            directory = Directory.GetParent(directory)?.FullName;
        }

        return Path.Combine(Directory.GetCurrentDirectory(), ".test-videos");
    }
}
