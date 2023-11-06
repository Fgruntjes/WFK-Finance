using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace App.Backend.Test.Logging;

public class XUnitLoggerProvider : ILoggerProvider
{
    private readonly IMessageSink _output;

    public XUnitLoggerProvider(IMessageSink output)
    {
        _output = output;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(_output, categoryName);
    }

    public void Dispose()
    {
    }
}
