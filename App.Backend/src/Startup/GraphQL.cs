using GraphQL.AspNet.Configuration;

namespace App.Backend.Startup;

public static class GraphQL
{
	public static void AddGraphQL(this IServiceCollection services, IHostEnvironment environment)
	{
		// GraphQL configuration
		services.AddGraphQL(o =>
		{
			o.ExecutionOptions.ResolverIsolation = ResolverIsolationOptions.ControllerActions;

			if (environment.IsDevelopment() || environment.IsStaging())
			{
				o.ExecutionOptions.DebugMode = true;				
			}
		});
	}
}