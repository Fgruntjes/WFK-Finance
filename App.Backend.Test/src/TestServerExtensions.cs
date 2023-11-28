using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Snapshooter.Core;

namespace App.Backend.Test;

public static class TestServerExtensions
{
    private static readonly ISnapshotFullNameReader _testNameResolver = new XunitSnapshotFullNameReader();

    public static async Task<string> ExecuteQuery<TSchema>(
        this TestServer<TSchema> Server,
        string? queryFile,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var context = Server.BuildQueryContext(queryFile, variables);
        return await Server.RenderResult(context);
    }

    public static async Task<string> ExecuteQuery<TSchema>(
        this TestServer<TSchema> Server,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var context = Server.BuildQueryContext(null, variables);
        return await Server.RenderResult(context);
    }

    private static QueryExecutionContext BuildQueryContext<TSchema>(
        this TestServer<TSchema> Server,
        string? queryFile,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var queryText = LoadQueryFile(queryFile);
        var queryBuilder = Server.CreateQueryContextBuilder();
        queryBuilder.AddQueryText(queryText);

        if (variables != null)
        {
            queryBuilder.AddVariableData(variables);
        }

        return queryBuilder.Build();
    }

    private static string LoadQueryFile(string? queryFile)
    {
        var queryFullName = _testNameResolver.ReadSnapshotFullName();
        queryFile ??= Path.Combine(queryFullName.FolderPath, "__graphql__", queryFullName.Filename + ".graphql");
        if (!File.Exists(queryFile))
        {
            File.Create(queryFile).Dispose();
        }

        var queryText = File.ReadAllText(queryFile);
        return queryText;
    }

}
