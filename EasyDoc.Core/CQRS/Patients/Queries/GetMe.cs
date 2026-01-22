using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Patients.Queries;

public record GetMeQuery : IQuery<PatientMeResponse>;

public record PatientMeResponse(Guid Id,
    string PersonName,
    string PhoneNumber,
    string email);


internal class GetMeQueryHandler : IQueryHandler<GetMeQuery, PatientMeResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetMeQueryHandler(IReadOnlyApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<PatientMeResponse>> HandleAsync(GetMeQuery query, CancellationToken cancellationToken = default)
    {
        var patientId = _userContext.PatientId;

        return await _dbContext.Patients
            .Where(p => p.Id == patientId)
            .Join(_dbContext.UserDtos,
            p => p.UserId,
            u => u.UserId,
            (p, u) => new PatientMeResponse(p.Id, p.PersonName, p.PhoneNumber.Value, u.Email))
            .FirstOrDefaultAsync(cancellationToken) ?? throw new CurrentUserNotFoundException(_userContext.UserId);
    }
}