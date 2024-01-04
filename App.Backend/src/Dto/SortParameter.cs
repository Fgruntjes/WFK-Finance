namespace App.Backend.Dto;

public class SortParameter
{
    public SortParameter()
    {
    }

    public SortParameter(string field, SortDirection direction)
    {
        Field = field;
        Direction = direction;
    }

    public string Field { get; set; } = "createdAt";
    public SortDirection Direction { get; set; } = SortDirection.Asc;
}