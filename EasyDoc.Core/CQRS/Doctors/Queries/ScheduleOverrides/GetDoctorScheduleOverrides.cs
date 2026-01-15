using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

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
    public Task<Result<IReadOnlyCollection<DoctorScheduleOverrideResponse>>> Handle(GetDoctorScheduleOverridesQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}