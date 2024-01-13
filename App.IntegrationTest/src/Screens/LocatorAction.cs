using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

public class LocatorAction
{
    public ILocator Locator { get; }

    public Func<ILocator, Task> Action { get; }

    public LocatorWaitForOptions LocatorWaitOptions { get; set; }

    public LocatorAction(ILocator locator, Func<ILocator, Task> action, LocatorWaitForOptions? locatorWaitOptions = default)
    {
        Locator = locator;
        Action = action;
        LocatorWaitOptions = locatorWaitOptions != default ? locatorWaitOptions : new LocatorWaitForOptions();
    }

    public LocatorAction(ILocator locator, LocatorWaitForOptions? locatorWaitOptions = default)
        : this(locator, _ => Task.CompletedTask, locatorWaitOptions)
    { }
}
