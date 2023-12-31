namespace App.Backend.Dto;

public class RangeParameter
{
	public int Start { get; set; } = 0;
	public int End { get; set; } = 25;
	
	public int Offset => Start;
	public int Limit => End - Start;
}