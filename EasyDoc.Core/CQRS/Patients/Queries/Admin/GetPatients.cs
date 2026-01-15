using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using FluentValidation;

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
