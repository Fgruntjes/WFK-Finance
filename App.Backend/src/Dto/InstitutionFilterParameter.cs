namespace App.Backend.Dto;

public class InstitutionFilterParameter : FilterParameter
{
    public required string Country
    {
        get => this["country"];
        set
        {
            this["country"] = value;
        }
    }
}