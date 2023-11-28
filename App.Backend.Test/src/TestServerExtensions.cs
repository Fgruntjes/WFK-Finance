using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Snapshooter.Core;

namespace App.Backend.Test;

public static class TestServerExtensions
{
    private static readonly ISnapshotFullNameReader _testNameResolver = new XunitSnapshotFullNameReader();

    public static async Task<string> ExecuteQuery<TSchema>(
        this TestServer<TSchema> server,
        string? queryFile,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var context = server.BuildQueryContext(queryFile, variables);
        return await server.RenderResult(context);
    }

    public static async Task<string> ExecuteQuery<TSchema>(
        this TestServer<TSchema> server,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var context = server.BuildQueryContext(null, variables);
        return await server.RenderResult(context);
    }

    public static QueryExecutionContext BuildQueryContext<TSchema>(
        this TestServer<TSchema> server,
        string? queryFile,
        object? variables = null)
        where TSchema : GraphSchema
    {
        var queryText = LoadQueryFile(queryFile);
        var queryBuilder = server.CreateQueryContextBuilder();
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
