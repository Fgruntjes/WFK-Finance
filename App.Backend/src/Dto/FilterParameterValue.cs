namespace App.Backend.Dto;

public class FilterParameterValue
{
    public string? StringValue { get; set; }
    public ICollection<string>? StringCollectionValue { get; set; }
    public int? IntValue { get; set; }
    public ICollection<int>? IntCollectionValue { get; set; }

    public static implicit operator FilterParameterValue(string value)
        => new() { StringValue = value };

    public static implicit operator FilterParameterValue(string[] value)
        => new() { StringCollectionValue = value.ToList() };

    public static implicit operator FilterParameterValue(int value)
        => new() { IntValue = value };

    public static implicit operator FilterParameterValue(int[] value)
        => new() { IntCollectionValue = value.ToList() };
}
