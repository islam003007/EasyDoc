using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Queries;

public record GetDoctorAppointmentsQuery(int PageNumber = 1, int PageSize = PageConstants.DefaultPageSize)
    : IQuery<IReadOnlyList<AppointmentResponse>>;

internal class GetDoctorAppointmentsQueryValidator : AbstractValidator<GetDoctorAppointmentsQuery>
{
    public GetDoctorAppointmentsQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

internal class GetDoctorAppointmentQueryHandler : IQueryHandler<GetDoctorAppointmentsQuery, IReadOnlyList<AppointmentResponse>>
{
    public Task<Result<IReadOnlyList<AppointmentResponse>>> Handle(GetDoctorAppointmentsQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}