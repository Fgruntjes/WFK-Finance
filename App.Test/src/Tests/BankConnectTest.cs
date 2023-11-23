
using App.Test.Screens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using VMelnalksnis.NordigenDotNet;

namespace App.Test.Tests;

public class InstitutionConnectTest : IClassFixture<NordigenFixture>
{
    private readonly NordigenFixture _fixture;
    private readonly INordigenClient _nordigenClient;

    public InstitutionConnectTest(NordigenFixture fixture)
    {
        _fixture = fixture;
        _nordigenClient = _fixture.Services.GetRequiredService<INordigenClient>();
    }

    [Fact]
    public async void ConnectUnauthorized()
    {
        var page = await _fixture.WithPage();

        var institutionConnectScreen = new InstitutionConnectScreen(page);

        await institutionConnectScreen.ClickMenuAsync();
        await institutionConnectScreen.ClickAddAsync();

        // Add new institution
        await page.GetByLabel("Select country").SelectOptionAsync("NL");
        await page.GetByLabel("Select your bank").SelectOptionAsync(new SelectOptionValue() { Label = "TEST_INSTITUTION" });
        await page.GetByRole(AriaRole.Button, new() { Name = "Connect" }).ClickAsync();

        // Follow nordigen flow
        await new List<LocatorAction>()
        {
            new (
                page.GetByRole(AriaRole.Button, new() { Name = "I agree" }),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByText("Approve"),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByTestId("activeroute:/institutionconnections/create-return")
            )
        }.GoToLastAsync();

        // On return expect test account linked
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();

        // Return to list
        await page.GetByText("Return to list").ClickAsync();

        // Ensure we are on the list page and see the connected account
        await institutionConnectScreen.AssertIsOnScreen();
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();
    }
}