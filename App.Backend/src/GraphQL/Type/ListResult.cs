using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class ListResult<T>
{
	[GraphField(TypeExpression = "[T!]!")]
	public IReadOnlyList<T> Items { get; set; }
	public int TotalCount { get; set; }

	public ListResult()
	{
		Items = new List<T>();
	}
}