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
        int screensTried = 0;
        do
        {
            currentIndex = await actions
                .Select(a => a.Locator)
                .FirstAvailableIndexAsync();

            await actions[currentIndex].Action(actions[currentIndex].Locator);
            screensTried++;

            if (screensTried > actions.Length)
            {
                throw new Exception("Tried {screensTried} times, but never reached last screen.")
                {
                    Data = { { "ScreensTried", screensTried } }
                };
            }

        } while (currentIndex < actions.Length - 1);
    }

    public static async Task<int> FirstAvailableIndexAsync(this IEnumerable<ILocator> locators)
    {
        return await locators.ToArray().FirstAvailableIndexAsync();
    }

    public static async Task<int> FirstAvailableIndexAsync(this ILocator[] locators)
    {
        var selectorTasks = locators
            .Select(x => x.WaitForAsync())
            .ToList();

        var finishedTask = await Task.WhenAny(selectorTasks);
        for (var index = 0; index < locators.Length; index++)
        {
            if (selectorTasks[index] == finishedTask)
            {
                return index;
            }
        }

        throw new Exception("None of the screens were found.");
    }
}
