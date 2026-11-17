using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Auth.Queries;

public record GetMeQuery : IQuery<AuthMeResponse>;

public record AuthMeResponse(
    Guid UserId,
    string Email,
    string Role,
    string DisplayName,
    Guid? PatientId,
    Guid? DoctorId);

internal class GetMeQueryHandler : IQueryHandler<GetMeQuery, AuthMeResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetMeQueryHandler(IReadOnlyApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<AuthMeResponse>> HandleAsync(GetMeQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;
        var role = AuthRoleResolver.Resolve(_userContext.Roles);

        var user = await _dbContext.UserReadModels
            .Where(user => user.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        var doctor = await _dbContext.Doctors
            .Where(doctor => doctor.UserId == userId)
            .Select(doctor => new { doctor.Id, doctor.PersonName })
            .FirstOrDefaultAsync(cancellationToken);

        var patient = await _dbContext.Patients
            .Where(patient => patient.UserId == userId)
            .Select(patient => new { patient.Id, patient.PersonName })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null && doctor is null && patient is null)
            throw new CurrentUserNotFoundException(userId);

        var displayName = role == "doctor" && doctor is not null
            ? doctor.PersonName
            : patient?.PersonName ?? doctor?.PersonName ?? user?.Email ?? userId.ToString();

        return new AuthMeResponse(
            userId,
            user?.Email ?? string.Empty,
            role,
            displayName,
            patient?.Id,
            doctor?.Id);
    }
}
