namespace App.Backend.Dto;

public class SortParameter
{
	public string Field { get; set; } = "id";
	public SortOrder Order { get; set; } = SortOrder.Asc;
}