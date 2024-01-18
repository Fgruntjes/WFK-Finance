using App.Institution;
using App.IntegrationTest.Screens;
using App.Lib.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using App.Lib.Configuration.Options;
using App.Lib.Data;
using App.Lib.Test.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.IntegrationTest;

public class AppFixture<TestType> : IAsyncLifetime
{
    public Database<DatabaseContext> Database { get; }
    public IServiceProvider Services { get; }

    private static bool IsCiCd => Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private readonly TestContext _testContext;

    public AppFixture(ILoggerProvider loggerProvider, TestContext testContext)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseConfiguration()
            .UseDatabase()
            .UseInstitution()
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
        _testContext = testContext;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_browserContext != null)
        {
            if (IsCiCd)
            {
                Console.WriteLine("Saving test trace");
                await _browserContext.Tracing.StopAsync(new()
                {
                    Path = Path.Combine(
                        GetTestTraceFolder(),
                        $"{_testContext.GetTestName<TestType>()}.zip"
                    )
                });
            }
            await _browserContext.DisposeAsync();
        }


        if (_browser != null)
            await _browser.DisposeAsync();

        _playwright?.Dispose();
    }

    public async Task<IPage> WithPage()
    {
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = IsCiCd,
        });

        _browserContext ??= await _browser.NewContextAsync(new()
        {
            RecordVideoDir = IsCiCd ? GetTestTraceFolder() : null,
            ScreenSize = new()
            {
                Height = 1080,
                Width = 1920,
            },
        });
        await _browserContext.Tracing.StartAsync(new()
        {
            Screenshots = false,
            Snapshots = true,
            Sources = true,
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
            new (homeScreen.Locator, new() { State = WaitForSelectorState.Attached })}
            .GoToLastAsync();
    }

    private string GetTestTraceFolder()
    {
        return Path.Combine(_testContext.ProjectRoot, ".test-traces");
    }
}
