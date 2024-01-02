namespace App.Backend.Dto;

public class SortParameter
{
	public required string Field { get; set; } = "id";
	public required SortOrder Order { get; set; } = SortOrder.Asc;
}