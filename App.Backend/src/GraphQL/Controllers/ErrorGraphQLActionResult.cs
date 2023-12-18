using GraphQL.AspNet.Execution;
using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Interfaces.Controllers;

namespace App.Backend.GraphQL.Controllers;

internal class ErrorGraphQlActionResult : IGraphActionResult
{
    private readonly string _message;
    private readonly string? _code;
    private readonly Exception? _exception;
    public IDictionary<string, object> MetaData { get; }

    public ErrorGraphQlActionResult(Exception exception, string? code)
        : this(exception.Message, code, exception)
    {
        foreach (var key in exception.Data.Keys)
        {
            if (key is not string stringKey)
                continue;

            var value = exception.Data[key]?.ToString() ?? string.Empty;
            MetaData.Add(stringKey, value);
        }
    }

    public ErrorGraphQlActionResult(string message, string? code = null, Exception? exception = null)
    {
        _message = message;
        _code = code;
        _exception = exception;
        MetaData = new Dictionary<string, object>(); ;
    }

    public Task CompleteAsync(SchemaItemResolutionContext resolutionContext)
    {
        var message = new GraphExecutionMessage(
            GraphMessageSeverity.Critical,
            _message,
            _code,
            resolutionContext.Request.Origin,
            _exception);

        foreach (var pair in MetaData)
        {
            message.MetaData.Add(pair);
        }

        resolutionContext.Messages.Add(message);
        return Task.CompletedTask;
    }
}