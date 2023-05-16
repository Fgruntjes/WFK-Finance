namespace App.Backend.Auth;

public sealed class Auth0Options
{
    public string Domain { get; set; } = null!;
    public string FullDomain => $"https://{Domain}/";
    public string Audience { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string Scope { get; set; } = null!;
    public string[] ScopeList => Scope.Split(' ');
    public string ClientSecret { get; set; } = null!;
}