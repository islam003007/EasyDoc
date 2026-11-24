using System.ComponentModel.DataAnnotations;

namespace EasyDoc.Application.Options;

public class DemoResetOptions
{
    public const string SectionName = "Demo:Reset";

    public bool Enabled { get; set; }

    [Range(1, int.MaxValue)]
    public int IntervalHours { get; set; } = 24;
}
