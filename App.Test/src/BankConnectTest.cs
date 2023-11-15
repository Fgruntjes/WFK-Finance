
using Microsoft.Playwright;

namespace App.Test;

public class BankConnectTest : IClassFixture<PlaywrightFixture>
{
    private PlaywrightFixture _fixture;

    public BankConnectTest(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void Test1()
    {
        var page = await _fixture.WithPage();

        // From institution list add new
        await page.GetByText("Bank accounts").ClickAsync();
        await page.GetByText("Add").ClickAsync();

        // Add new institution
        await page.GetByLabel("Select country").SelectOptionAsync("NL");
        await page.GetByLabel("Select your bank").SelectOptionAsync(new SelectOptionValue() { Label = "TEST_INSTITUTION" });
        await page.GetByRole(AriaRole.Button, new () { Name = "Connect" }).ClickAsync();

        // Follow nordigen flow
        var nordigenAgreeSelector = page.GetByRole(AriaRole.Button, new () { Name = "I agree" });
        var accountReturnSelector = page.GetByText("Connected bank account(s)");

        var nordigenAgreePage = nordigenAgreeSelector.WaitForAsync();
        var accountReturnPage = accountReturnSelector.WaitForAsync();
        var returnPage = await Task.WhenAny(nordigenAgreePage, accountReturnPage);
        if (returnPage == nordigenAgreePage)
        {
            await nordigenAgreeSelector.ClickAsync();
            await page.GetByRole(AriaRole.Button, new () { Name = "Sign in" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new () { Name = "Approve" }).ClickAsync();
        }

        // On return expect test account linked
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();

        // Return to list
        await page.GetByText("Return to list").ClickAsync();
        
        // Ensure we are on the list page and see the connected account
        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Bank connections" })).ToBeVisibleAsync();
        await Assertions.Expect(page.GetByText("GL7839380000039382")).ToBeVisibleAsync();
    }
}