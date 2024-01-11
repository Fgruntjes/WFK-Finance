using Microsoft.Playwright;

namespace App.IntegrationTest;

internal static class IPageExtension
{
    public static async Task ClickMenuAsync(this IPage page, string route)
    {
        await page.Locator($"[role='menuitem'] >> [href='{route}']").ClickAsync();
    }

    public static async Task SearchSelectOptionAsync(this IPage page, string fieldId, string value)
    {
        await page.ClickAsync($"#{fieldId}");
        await page.Locator($"#{fieldId}_list + .rc-virtual-list >> [title*='{value}']").ClickAsync();
    }
}
