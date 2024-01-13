using App.IntegrationTest.Screens;
using Microsoft.Playwright;

namespace App.IntegrationTest.Tests;

public class InstitutionConnectTest : IClassFixture<NordigenFixture>
{
    private readonly NordigenFixture _fixture;

    public InstitutionConnectTest(NordigenFixture fixture)
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

        // Keep refreshing until import status is Success
        await page.DoAndWait(
            page.GetByTestId("import-status-badge").GetByText("Success"),
            (page) => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Refresh" }).ClickAsync());

        await page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Transactions" }).ClickAsync();

        // Check if we see the transaction
        var transactions = await page.GetByText("PAYMENT Alderaan Coffe").AllAsync();
        await Assertions.Expect(transactions[0]).ToBeVisibleAsync();
    }
}