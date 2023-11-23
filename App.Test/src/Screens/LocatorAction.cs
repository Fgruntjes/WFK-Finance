using Microsoft.Playwright;

namespace App.Test.Screens;

public class LocatorAction
{
    public ILocator Locator { get; }

    public Func<ILocator, Task> Action { get; }

    public LocatorAction(ILocator locator, Func<ILocator, Task> action)
    {
        Locator = locator;
        Action = action;
    }

    public LocatorAction(ILocator locator) : this(locator, _ => Task.CompletedTask)
    { }
}
