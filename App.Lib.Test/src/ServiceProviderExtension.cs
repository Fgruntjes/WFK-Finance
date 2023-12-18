using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace App.Lib.Test;

public static class ServiceProviderExtension
{
    public static void MockTransient<T>(this IServiceCollection services)
        where T : class
    {
        var mock = new Mock<T>();
        services.AddTransient(_ => mock.Object);
        services.AddSingleton(mock);
    }

    public static void MockScoped<T>(this IServiceCollection services)
        where T : class
    {
        var mock = new Mock<T>();
        services.AddScoped(_ => mock.Object);
        services.AddSingleton(mock);
    }

    public static void MockSingleton<T>(this IServiceCollection services)
        where T : class
    {
        var mock = new Mock<T>();
        services.AddSingleton(mock.Object);
        services.AddSingleton(mock);
    }

    public static void WithMock<T>(this IServiceProvider services, Action<Mock<T>> action)
        where T : class
    {
        var mock = services.GetRequiredService<Mock<T>>();
        action(mock);
    }
}