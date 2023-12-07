using System.ComponentModel.DataAnnotations;

namespace App.Lib.Configuration.Options;

public class DataProtectionOptions
{
    public const string Section = "DataProtection";
    [Required]
    public bool Enabled { get; set; } = true;
    [Required]
    public string KeyVaultUri { get; set; } = null!;
    [Required]
    public string KeyName { get; set; } = null!;
    [Required]
    public string StorageAccountUri { get; set; } = null!;
    [Required]
    public string StorageContainer { get; set; } = null!;
    [Required]
    public string KeyBlobName { get; set; } = null!;
}