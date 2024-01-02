namespace App.Backend.Dto;

public class RangeParameter
{
	public RangeParameter()
	{ }

	public RangeParameter(int start, int end)
	{
		Start = start;
		End = end;
	}

	public int Start { get; set; } = 0;
	public int End { get; set; } = 25;

	public int Offset => Start;
	public int Limit => End - Start;
}
