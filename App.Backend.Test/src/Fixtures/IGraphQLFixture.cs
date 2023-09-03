using System.Text.Json.Nodes;
using App.Backend.Data;
using GraphQL.AspNet.Interfaces.Execution;

namespace App.Backend.Test.Fixtures;

public interface IGraphQLFixture
{
	DatabaseContext Database { get; }
	Task<string> ExecuteQuery(string? queryFile, object? variables = null);
	Task<string> ExecuteQuery(object? variables = null);
}