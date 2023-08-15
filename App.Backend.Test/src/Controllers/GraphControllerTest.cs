using System.Text.Json.Nodes;
using App.Backend.Data;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Execution.Contexts;
using GraphQL.AspNet.Interfaces.Execution;
using GraphQL.AspNet.Schemas;
using GraphQL.AspNet.Tests.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Backend.Test.Controllers;

public class GraphControllerTest<TController>
	where TController : GraphController
{
	protected QueryContext CreateQueryContext()
	{
		var services = new ServiceCollection();
		services.AddDbContext<DatabaseContext>(options =>
		{
			options.UseInMemoryDatabase("Test");
		});
		
		var builder = new TestServerBuilder<GraphSchema>(serviceCollection: services);
		builder.AddGraphQL(o => o.AddController<TController>());
		
		return new QueryContext()
		{
			Services = services,
			Builder = builder,
		};
	}

	protected class QueryContext
	{
		public IServiceCollection Services { get; init; } = null!;
		public TestServerBuilder<GraphSchema> Builder { get; init; } = null!;

		private TestServer<GraphSchema>? _server;
		public TestServer<GraphSchema> Server
		{
			get { return _server ??= Builder.Build(); }
		}

		public DatabaseContext Database => Server.ServiceProvider.GetRequiredService<DatabaseContext>();

		public async Task<JsonNode> Render(string queryText, object variables = null)
		{
			var context = BuildQueryContext(queryText, variables);
			var result = await Server.RenderResult(context);
			return JsonNode.Parse(result) ?? throw new InvalidOperationException("Failed to parse result");
		}


		public async Task<IQueryExecutionResult> Execute(string queryText, object variables = null)
		{
			var context = BuildQueryContext(queryText, variables);
			await Server.ExecuteQuery(context);
			
			return context.Result;
		}

		private QueryExecutionContext BuildQueryContext(string queryText, object variables = null)
		{
			var queryBuilder = Server.CreateQueryContextBuilder();
			queryBuilder.AddQueryText(queryText);
			if (variables != null)
			{
				queryBuilder.AddVariableData(variables);
			}
			
			return queryBuilder.Build();
		}
	}
}