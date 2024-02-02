using System.Diagnostics;
using Xunit.Sdk;

namespace App.Lib.Test;

public static class AssertHelper
{
    public static async Task AssertRetry(Action assertAction)
    {
        await AssertRetry(assertAction, TimeSpan.FromSeconds(5));
    }

    public static async Task AssertRetry(Action assertAction, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        Exception? lastException = null;

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                assertAction();
                return;
            }
            catch (Exception exception)
            {
                if (exception is not IAssertionException)
                    throw;

                lastException = exception;
            }

            await Task.Delay(10);
        }

        if (lastException != null)
            throw lastException;

        throw new TimeoutException("Assertion did not pass within the specified timeout and no exception was thrown.");
    }
}