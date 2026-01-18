using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries.ScheduleOverrides;

public record GetDoctorScheduleOverridesQuery(Guid DoctorId) : IQuery<IReadOnlyCollection<DoctorScheduleOverrideResponse>>;

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
    IReadOnlyCollection<DoctorScheduleOverrideResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorScheduleOverridesQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyCollection<DoctorScheduleOverrideResponse>>> Handle(GetDoctorScheduleOverridesQuery query, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Where(d => d.Id == query.DoctorId)
            .SelectMany(d => d.ScheduleOverrides)
            .Select(s => new DoctorScheduleOverrideResponse(s.Id, s.Date, s.IsAvailable, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);
    }
}