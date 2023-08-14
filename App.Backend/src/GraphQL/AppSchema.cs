using GraphQL.Types;

namespace App.Backend.GraphQL;

public class AppSchema : Schema
{
	public AppSchema(IServiceProvider provider, AppQuery query) : base(provider)
	{
		Query = query;
	}
}