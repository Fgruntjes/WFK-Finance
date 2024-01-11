using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

public static class LocatorExtensions
{
    public static async Task GoToLastAsync(this IEnumerable<LocatorAction> actions)
    {
        await actions.ToArray().GoToLastAsync();
    }

    public static async Task GoToLastAsync(this LocatorAction[] actions)
    {
        int currentIndex;
        var timeout = DateTime.Now.AddSeconds(30);
        do
        {
            var locatorTasks = actions.Select(a => a.Locator.WaitForAsync()).ToArray();
            currentIndex = Task.WaitAny(locatorTasks);
            if (locatorTasks[currentIndex].IsFaulted)
            {
                Task.WaitAll(locatorTasks);
            }

            await actions[currentIndex].Action(actions[currentIndex].Locator);
            if (timeout < DateTime.Now)
            {
                throw new Exception("Tried for 30 seconds, but never reached last screen.");
            }
        } while (currentIndex < actions.Length - 1);
    }

    private static async Task<bool> DidFindLocator(ILocator locator)
    {
        try
        {
            await locator.WaitForAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
