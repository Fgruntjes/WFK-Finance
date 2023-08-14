using GraphQL.Types;

namespace App.Backend.GraphQL;

public class AppQuery : ObjectGraphType
{
	public AppQuery()
	{
		Name = "Query";
		
		Field<StringGraphType>(
			name: "hello",
			resolve: context => "world"
		);
	}
}