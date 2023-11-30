using System.Text.Json;
using GraphQL.AspNet.Tests.Framework.PipelineContextBuilders;

namespace App.Backend.Test;

public static class QueryContextBuilderExtensions
{
    public static QueryContextBuilder AddVariableData<TValue>(this QueryContextBuilder builder, TValue value)
    {
        return builder.AddVariableData(JsonSerializer.Serialize(value));
    }
}