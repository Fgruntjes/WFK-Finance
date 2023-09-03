namespace App.Backend.GraphQL.Type;

public class SkipLimitArgs
{
	public int Skip { get; set; } = 0;
	public int Limit { get; set; } = 25;
}