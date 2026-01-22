using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries.Schedules;

public record GetDoctorSchedulesQuery(Guid DoctorId) : IQuery<IReadOnlyList<DoctorScheduleResponse>>;

public record class DoctorScheduleResponse(Guid Id, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);

internal class GetDoctorSchedulesQueryValidator : AbstractValidator<GetDoctorSchedulesQuery>
{
    public GetDoctorSchedulesQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class GetDoctorSchedulesQueryHandler : IQueryHandler<GetDoctorSchedulesQuery, IReadOnlyList<DoctorScheduleResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorSchedulesQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<DoctorScheduleResponse>>> HandleAsync(GetDoctorSchedulesQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Where(d => d.Id == query.DoctorId)
            .SelectMany(d => d.Schedules)
            .Select(s => new DoctorScheduleResponse(s.Id, s.DayOfWeek, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);
    }
}