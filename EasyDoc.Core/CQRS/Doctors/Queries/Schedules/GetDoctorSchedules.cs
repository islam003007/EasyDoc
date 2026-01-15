using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Queries.Schedules;

public record GetDoctorSchedulesQuery(Guid DoctorId) : IQuery<IReadOnlyCollection<DoctorScheduleResponse>>;

public record class DoctorScheduleResponse(Guid Id, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);

internal class GetDoctorSchedulesQueryValidator : AbstractValidator<GetDoctorSchedulesQuery>
{
    public GetDoctorSchedulesQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class GetDoctorSchedulesQueryHandler : IQueryHandler<GetDoctorSchedulesQuery, IReadOnlyCollection<DoctorScheduleResponse>>
{
    public Task<Result<IReadOnlyCollection<DoctorScheduleResponse>>> Handle(GetDoctorSchedulesQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}