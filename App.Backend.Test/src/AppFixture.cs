using App.Data;
using App.Backend.Startup;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Core;
using Xunit.Abstractions;
using GraphQL.AspNet.Configuration;
using Xunit.DependencyInjection.Logging;

namespace App.Backend.Test;

public class AppFixture<TController>
	where TController : GraphController
{
	private readonly ISnapshotFullNameReader _testNameResolver;
	private readonly TestServerBuilder<GraphSchema> _builder;
	private TestServer<GraphSchema>? _server;

	public TestServer<GraphSchema> Server => _server ??= _builder.Build();
	public DatabaseContext Database => Server.ServiceProvider.GetRequiredService<DatabaseContext>();
	public Guid OrganisationId => Server.ServiceProvider.GetRequiredService<AppHttpContext>().OrganisationId();

	public AppFixture(IMessageSink logMessageSink)
	{
		_testNameResolver = new XunitSnapshotFullNameReader();

		var builder = new TestServerBuilder<GraphSchema>();
		// Allow other fixutres to register mocks
		RegisterMocks(builder);

		// Pass logging to xunit output
		builder.AddLogging(loggingBuilder =>
		{
			loggingBuilder.AddXunitOutput();
		});

		// Load database
		builder.AddDbContext<DatabaseContext>(options =>
		{
			options.UseInMemoryDatabase(Guid.NewGuid().ToString());
		});

		// App configuration
		builder.AddAppServices();
		builder.AddGraphQL(options =>
		{
			ConfigureGraphQL(options);
		});

		// TODO handle non authorized test cases
		builder.UserContext.Authenticate();

		_builder = builder;
	}

	public async Task<string> ExecuteQuery(string? queryFile, object? variables = null)
	{
		var context = BuildQueryContext(queryFile, variables);
		return await Server.RenderResult(context);
	}

	public async Task<string> ExecuteQuery(object? variables = null)
	{
		var context = BuildQueryContext(null, variables);
		return await Server.RenderResult(context);
	}

	protected virtual void RegisterMocks(IServiceCollection services)
	{

	}

	protected virtual void ConfigureGraphQL(SchemaOptions options)
	{
		options.AddController<TController>();
		options.ResponseOptions.ExposeExceptions = true;
	}

	private QueryExecutionContext BuildQueryContext(string? queryFile, object? variables = null)
	{
		var queryText = LoadQueryFile(queryFile);
		var queryBuilder = Server.CreateQueryContextBuilder();
		queryBuilder.AddQueryText(queryText);
		//queryBuilder.AddUserSecurityContext();
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