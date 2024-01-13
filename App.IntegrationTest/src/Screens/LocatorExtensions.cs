using Microsoft.Playwright;

namespace App.IntegrationTest.Screens;

public static class LocatorExtensions
{
    public static async Task GoToLastAsync(this IEnumerable<LocatorAction> actions)
    {
        await actions.ToArray().GoToLastAsync();
    }

    public static async Task GoToLastAsync(this LocatorAction[] actions, int maxTries = 10)
    {
        var tries = 0;
        var currentIndex = 0;
        do
        {
            var locatorTasks = actions.Select(a => a.Locator.WaitForAsync(a.LocatorWaitOptions)).ToArray();
            currentIndex = Task.WaitAny(locatorTasks);
            if (locatorTasks[currentIndex].IsFaulted)
            {
                // None of the locators matched, wait all tasks we get a combined exception.
                Task.WaitAll(locatorTasks);
            }

            await actions[currentIndex].Action(actions[currentIndex].Locator);
            if (tries++ >= maxTries)
            {
                // ended in loop while rerunning same actions, wait all tasks so we get a combined exception
                Task.WaitAll(locatorTasks);

                // If somehow all tasks finished sucesfully throw to not end up in permanent loop
                throw new Exception("All locator tasks finished successfully");
            }
        } while (currentIndex != actions.Length - 1);
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
