using EasyDoc.Application.Constants;

namespace EasyDoc.Application.CQRS.Auth.Queries;

public static class AuthRoleResolver
{
    // A user can hold more than one role (for example a doctor and a patient profile).
    // The demo UI expects a single role, so prefer the broadest one.
    public static string Resolve(IReadOnlyCollection<string> roles)
    {
        if (roles.Contains(Roles.Admin))
            return "admin";

        if (roles.Contains(Roles.Doctor))
            return "doctor";

        if (roles.Contains(Roles.Patient))
            return "patient";

        return roles.FirstOrDefault()?.ToLowerInvariant() ?? string.Empty;
    }
}
