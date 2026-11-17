namespace EasyDoc.Application.Options;

public class DemoOptions
{
    // Demo endpoints are disabled unless explicitly enabled by configuration.
    public bool Enabled { get; set; } = false;
}
