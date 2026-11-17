using EasyDoc.Application.Constants;

namespace EasyDoc.Application.Demo;

public static class DemoRoles
{
    public const string Patient = "patient";
    public const string Doctor = "doctor";
    public const string Admin = "admin";
}

public sealed record DemoUserProfile(string Key, string Label, string Role, string Email);

// Server-side allowlist for demo sign-in. The client never supplies an email or a password.
public static class DemoUserCatalog
{
    public static IReadOnlyList<DemoUserProfile> All { get; } = new List<DemoUserProfile>
    {
        new("sara", "سارة محمد", DemoRoles.Patient, "sara@example.com"),
        new("ahmed", "أحمد حسن", DemoRoles.Doctor, "ahmed@example.com"),
        new("admin", "Administrator", DemoRoles.Admin, "admin@example.com")
    };

    public static DemoUserProfile? Find(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        key = key.Trim();

        return All.FirstOrDefault(user => string.Equals(user.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    // Maps the public demo role to the Identity role name used by authorization policies.
    public static string? GetIdentityRole(string? role) => role switch
    {
        DemoRoles.Patient => Roles.Patient,
        DemoRoles.Doctor => Roles.Doctor,
        DemoRoles.Admin => Roles.Admin,
        _ => null
    };
}
