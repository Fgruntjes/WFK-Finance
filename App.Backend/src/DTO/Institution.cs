using System;

namespace App.Backend.DTO;

public class Institution
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public Uri Logo { get; init; } = null!;
}
