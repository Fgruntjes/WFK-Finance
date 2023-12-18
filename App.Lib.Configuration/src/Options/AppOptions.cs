using System.ComponentModel.DataAnnotations;

namespace App.Lib.Configuration.Options;

public class AppOptions
{
    public const string Section = "App";
    [Required]
    public string FrontendUrl { get; set; } = null!;
    [Required]
    public string Environment { get; set; } = null!;
    [Required]
    public string Version { get; set; } = null!;
}