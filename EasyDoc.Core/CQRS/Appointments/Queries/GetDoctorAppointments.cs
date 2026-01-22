using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetDoctorAppointmentQueryHandler(IReadOnlyApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> HandleAsync(GetDoctorAppointmentsQuery query, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var appointments = await _dbContext.Appointments.Where(a => a.DoctorId == doctorId)
                                                        .Select(AppointmentMapper.ToAppointmentResponse)
                                                        .ToListAsync(cancellationToken);

        return appointments;
    }
}