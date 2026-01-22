using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using EasyDoc.Application.Errors;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Patients.Queries.Admin;

public record GetPatientByIdQuery(Guid PatientId) : IQuery<AdminPatientResponse>;

internal class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}

internal class GetPatientByIdQueryHandle : IQueryHandler<GetPatientByIdQuery, AdminPatientResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetPatientByIdQueryHandle(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<AdminPatientResponse>> HandleAsync(GetPatientByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Where(p => p.Id == query.PatientId)
            .Join(_dbContext.UserDtos,
            p => p.UserId,
            u => u.UserId,
            AdminPatientResponseMapper.ToPatientResponse)
            .FirstOrDefaultAsync(cancellationToken) ?? Result.Failure<AdminPatientResponse>(PatientErrors.NotFound(query.PatientId));
    }
}