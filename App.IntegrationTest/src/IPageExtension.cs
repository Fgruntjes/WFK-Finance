using Microsoft.Playwright;

namespace App.IntegrationTest;

internal static class IPageExtension
{
    public static ILocator GetByAppTestId(this IPage page, string testId)
    {
        return page.Locator($".apptest-{testId}");
    }
}