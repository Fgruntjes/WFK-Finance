using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;

namespace App.Backend.Tests;

public static class TestUtils
{
    private static List<Action> verifyActions = new List<Action>();

    public static void InitVerifyActions() => verifyActions = new List<Action>();

    public static void VerifyAllSetups()
    {
        foreach (var action in verifyActions)
        {
            action.Invoke();
        }
    }

    public static ISetup<T> SetupAndVerify<T>(this Mock<T> mock, Expression<Action<T>> expression, Times times) where T : class
    {
        verifyActions.Add(() => mock.Verify(expression, times));
        return mock.Setup(expression);
    }

    public static ISetup<T, TResult> SetupAndVerify<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression, Times times) where T : class
    {
        verifyActions.Add(() => mock.Verify(expression, times));
        return mock.Setup(expression);
    }
}