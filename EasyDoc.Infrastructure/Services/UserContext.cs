using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace EasyDoc.Infrastructure.Services;

internal class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid UserId 
    {
        get
        {
            string? userIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out Guid userId))
                throw new AppException("Users.NotAuthinticated", "The curent user is not authenticated");

            return userId;
        }
    }

    public Guid DoctorId
    {
        get
        {
            string? doctorIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(AppClaimTypes.DoctorProfileId);

            if (!Guid.TryParse(doctorIdValue, out Guid doctorId))
                throw new AppException("Users.NotAuthinicated", "The current user is not authenticated");

            return doctorId;
        }
    }

    public Guid PatientId
    {
        get
        {
            string? patientIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(AppClaimTypes.PatientProfileId);

            if (!Guid.TryParse(patientIdValue, out Guid patientProfileId))
                throw new AppException("Users.NotAuthinicated", "The current user is not authenticated");

            return patientProfileId;
        }
    }
}
