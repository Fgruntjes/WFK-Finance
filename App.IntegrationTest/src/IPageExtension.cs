using Microsoft.Playwright;

namespace App.IntegrationTest;

internal static class IPageExtension
{
    public static async Task ClickMenuAsync(this IPage page, string route, string? subMenuOf = null)
    {
        var menuItemLocator = page.Locator($"[role='menuitem'] >> [href='{route}']");
        if (subMenuOf == null)
        {
            await menuItemLocator.ClickAsync();
            return;
        }

        if (!await menuItemLocator.IsVisibleAsync())
        {
            await page.Locator($"[role='menuitem']:has-text('{subMenuOf}')").ClickAsync();
        }
        await menuItemLocator.ClickAsync();
    }

    public static async Task SearchSelectOptionAsync(this IPage page, string fieldId, string value)
    {
        await page.ClickAsync($"#{fieldId}");
        await page.Locator($"#{fieldId}_list + .rc-virtual-list >> [title*='{value}']").ClickAsync();
    }

    public static async Task DoAndWait(
        this IPage page,
        ILocator waitFor,
        Action<IPage> doAction,
        LocatorWaitForOptions? waitForOptions = null,
        TimeSpan timeout = default,
        TimeSpan interval = default)
    {
        timeout = timeout == default ? TimeSpan.FromSeconds(30) : timeout;
        interval = interval == default ? TimeSpan.FromSeconds(2) : interval;
        waitForOptions ??= new LocatorWaitForOptions()
        {
            State = WaitForSelectorState.Visible,
            Timeout = (float)timeout.TotalSeconds
        };

        var timeoutTask = Task.Delay(timeout);
        var waitTask = Task.CompletedTask;

        while (!timeoutTask.IsCompleted)
        {
            doAction(page);

            waitTask = waitFor.WaitForAsync(waitForOptions);

            if (await Task.WhenAny(waitTask, timeoutTask) == waitTask && waitTask.IsCompletedSuccessfully)
            {
                return;
            }

            await Task.Delay(interval);
        }

        var exceptions = new List<Exception>();
        if (timeoutTask.Exception != null)
            exceptions.Add(timeoutTask.Exception);
        if (waitTask.Exception != null)
            exceptions.Add(waitTask.Exception);

        throw new AggregateException(exceptions);
    }

    public static ILocator GetRowButton(this IPage page, string rowSelector, string buttonText, string? testId = null)
    {
        ILocator locator;
        if (testId != null)
        {
            locator = page.GetByTestId(testId);
        }
        else
        {
            locator = page.Locator("table");
        }

        return locator = locator
            .Locator($"tr:has-text('GL7839380000039382')")
            .GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Show" });
    }
}
