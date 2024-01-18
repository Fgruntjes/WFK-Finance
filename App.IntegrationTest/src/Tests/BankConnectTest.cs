using App.IntegrationTest.Screens;
using Microsoft.Playwright;

namespace App.IntegrationTest.Tests;

public class InstitutionConnectTest : IClassFixture<NordigenFixture<InstitutionConnectTest>>
{
    private readonly NordigenFixture<InstitutionConnectTest> _fixture;

    public InstitutionConnectTest(NordigenFixture<InstitutionConnectTest> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void ConnectUnauthorized()
    {
        var page = await _fixture.WithPage();
        var institutionConnectScreen = new InstitutionConnectScreen(page);

        await institutionConnectScreen.ClickMenuAsync();
        await institutionConnectScreen.ClickAddAsync();

        // Add new institution
        await page.SearchSelectOptionAsync("countryIso2", "Netherlands");
        await page.SearchSelectOptionAsync("institutionId", "TEST_INSTITUTION");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();

        // Follow nordigen flow
        await new List<LocatorAction>()
        {
            new (
                page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "I agree" }),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Sign in" }),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByText("Approve"),
                async (locator) => await locator.ClickAsync()
            ),
            new (
                page.GetByTestId("activeroute:/bank-accounts/create-return")
            )
        }.GoToLastAsync();

        // On return expect test account linked
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();

        // Return to list
        await page.GetByText("Return to list").ClickAsync();

        // Ensure we are on the list page and see the connected account
        await institutionConnectScreen.AssertIsOnScreen();
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();

        // Go to accounts show page and check if we see transactions
        await page.GetRowButton("GL7839380000039382", "Show", "institutionaccounts-table").ClickAsync();

        // Keep refreshing until import status is Success, timeout at 5 mintutes due to start time worker container
        await page.DoAndWait(
            page.GetByTestId("import-status-badge").GetByText("Success"),
            (page) => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Refresh" }).ClickAsync(),
            timeout: TimeSpan.FromMinutes(5));

        await page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Transactions" }).ClickAsync();

        // Check if we see the transaction
        await Assertions.Expect(page.Locator(":has-text('PAYMENT Alderaan Coffe')").Locator("nth=0")).ToBeVisibleAsync();
    }
}