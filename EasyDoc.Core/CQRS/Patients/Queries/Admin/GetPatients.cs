using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Doctors.Queries;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Patients.Queries.Admin;

public record GetPatientsQuery(int PageNumber = 1, int PageSize = PageConstants.DefaultPageSize) : IQuery<IReadOnlyList<AdminPatientResponse>>;

internal class GetPatientsQueryValidator : AbstractValidator<GetPatientsQuery>
{
    public GetPatientsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
           .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

internal class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, IReadOnlyList<AdminPatientResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetPatientsQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<AdminPatientResponse>>> HandleAsync(GetPatientsQuery query, CancellationToken cancellationToken = default)
    {
        var patients = await _dbContext.Patients
            .Join(_dbContext.UserDtos,
            p => p.UserId,
            u => u.UserId,
            (p, u) => new AdminPatientResponse(p.Id, u.UserId, p.PersonName, p.PhoneNumber.Value, p.IsDeleted, u.Email))
            .OrderBy(d => d.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return patients;
    }
}