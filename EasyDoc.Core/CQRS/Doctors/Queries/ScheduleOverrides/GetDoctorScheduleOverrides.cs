using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EasyDoc.Application.CQRS.Doctors.Queries.ScheduleOverrides;

public record GetDoctorScheduleOverridesQuery(Guid DoctorId) : IQuery<IReadOnlyList<DoctorScheduleOverrideResponse>>;

public record class DoctorScheduleOverrideResponse(Guid Id, DateOnly Date, bool IsAvaiable, TimeOnly? StartTime, TimeOnly? EndTime);

internal class GetDoctorScheduleOverridesQueryValidator : AbstractValidator<GetDoctorScheduleOverridesQuery>
{
    public GetDoctorScheduleOverridesQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class GetDoctorScheduleOverridesQueryHandler : IQueryHandler<GetDoctorScheduleOverridesQuery,
    IReadOnlyList<DoctorScheduleOverrideResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorScheduleOverridesQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<DoctorScheduleOverrideResponse>>> HandleAsync(GetDoctorScheduleOverridesQuery query,
        CancellationToken cancellationToken = default)
    {
        var doctorWithOverrides = await _dbContext.Doctors
            .Where(d => d.Id == query.DoctorId)
            .Select(d => new
            {
                Overrides = d.ScheduleOverrides
                    .Select(s => new DoctorScheduleOverrideResponse(
                        s.Id, s.Date, s.IsAvailable, s.StartTime, s.EndTime))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return doctorWithOverrides is not null ?
            doctorWithOverrides.Overrides :
            Result.Failure<IReadOnlyList<DoctorScheduleOverrideResponse>>(DoctorErrors.NotFound(query.DoctorId));
    }
}