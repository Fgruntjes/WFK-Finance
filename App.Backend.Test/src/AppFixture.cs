using App.Backend.Data;
using App.Backend.Startup;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Core;

namespace App.Backend.Test;

public class AppFixture<TController>
	where TController : GraphController
{
	private readonly ISnapshotFullNameReader _testNameResolver;
	private readonly TestServer<GraphSchema> _server;
	public DatabaseContext Database { get; }

	public AppFixture()
	{
		_testNameResolver = new XunitSnapshotFullNameReader();

		var services = new ServiceCollection();
		services.AddDbContext<DatabaseContext>(options =>
		{
			options.UseInMemoryDatabase(Guid.NewGuid().ToString());
		});
		services.AddAppServices();

		var builder = new TestServerBuilder<GraphSchema>(serviceCollection: services);
		builder.AddGraphQL(o =>
		{
			o.AddController<TController>();
			o.ResponseOptions.ExposeExceptions = true;
		});

		// TODO handle non authorized test cases
		builder.UserContext.Authenticate();

		_server = builder.Build();

		Database = _server.ServiceProvider.GetRequiredService<DatabaseContext>();
	}


	public async Task<string> ExecuteQuery(string? queryFile, object? variables = null)
	{
		var context = BuildQueryContext(queryFile, variables);
		return await _server.RenderResult(context);
	}

	public async Task<string> ExecuteQuery(object? variables = null)
	{
		var context = BuildQueryContext(null, variables);
		return await _server.RenderResult(context);
	}

	private QueryExecutionContext BuildQueryContext(string? queryFile, object? variables = null)
	{
		var queryText = LoadQueryFile(queryFile);
		var queryBuilder = _server.CreateQueryContextBuilder();
		queryBuilder.AddQueryText(queryText);
		if (variables != null)
		{
			queryBuilder.AddVariableData(variables);
		}

		return queryBuilder.Build();
	}

	private string LoadQueryFile(string? queryFile)
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