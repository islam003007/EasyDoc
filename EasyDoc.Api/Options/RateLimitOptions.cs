using System.ComponentModel.DataAnnotations;

namespace EasyDoc.Api.Options;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public bool Enabled { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int PermitLimit { get; set; } = 120;

    [Range(1, int.MaxValue)]
    public int WindowSeconds { get; set; } = 60;
}
