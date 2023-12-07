using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace App.Backend.Test;

internal static class ServiceCollectionExtensions
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
}