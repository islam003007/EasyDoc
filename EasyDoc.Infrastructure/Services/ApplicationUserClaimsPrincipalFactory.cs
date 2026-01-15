using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Constants;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EasyDoc.Infrastructure.Services;

internal class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private IReadOnlyApplicationDbContext _context;
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> options,
        IReadOnlyApplicationDbContext context) : base(userManager, roleManager, options)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var userId = user.Id;

        // so that a user can be both a doctor and a patient at the same time.
        // can be a single complicated query, however using 2 is much simpler. Awaiting one by one is safer for the dbcontext.
        Guid doctorId = await _context.Doctors.Where(d => d.UserId == userId).Select(d => d.Id).FirstOrDefaultAsync();
        Guid patientId = await _context.Patients.Where(p => p.UserId == userId).Select(d => d.Id).FirstOrDefaultAsync();

        if (doctorId != default)
        {
            identity.AddClaim(new Claim(AppClaimTypes.DoctorProfileId, doctorId.ToString()!));
        }

        if (patientId != default)
        {
            identity.AddClaim(new Claim(AppClaimTypes.PatientProfileId, patientId.ToString()!));
        }

        return identity;
    }
}
