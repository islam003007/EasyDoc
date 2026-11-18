namespace EasyDoc.Application.Options;

public class DemoOptions
{
    // Demo endpoints are disabled unless explicitly enabled by configuration.
    public bool Enabled { get; set; } = false;

    // Demo deployments can bypass email confirmation. This must stay false in production.
    public bool AutoConfirmRegistrations { get; set; } = false;
}
