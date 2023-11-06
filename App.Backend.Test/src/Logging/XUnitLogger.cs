using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace App.Backend.Test.Logging;

public class XUnitLogger : ILogger
{
    private readonly IMessageSink _output;
    private readonly string _categoryName;

    public XUnitLogger(IMessageSink output, string categoryName)
    {
        _output = output;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _output.OnMessage(new DiagnosticMessage($"[{DateTime.Now}] [{logLevel}] [{_categoryName}] {message}"));
    }
}