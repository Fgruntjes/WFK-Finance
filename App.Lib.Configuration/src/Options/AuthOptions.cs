using System.ComponentModel.DataAnnotations;

namespace App.Lib.Configuration.Options;

public class AuthOptions
{
    public const string Section = "Auth";
    [Required]
    public string Domain { get; set; } = null!;
    [Required]
    public string Audience { get; set; } = null!;
}