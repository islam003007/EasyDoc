using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Patients.Queries.Admin;

public record GetPatientByIdQuery(Guid PatientId) : IQuery<AdminPatientResponse>;

internal class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
    }
}
