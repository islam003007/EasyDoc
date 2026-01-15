using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Domain.Entities.PatientAggregate;
using System.Linq.Expressions;

namespace EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;

public record AdminPatientResponse(Guid Id,
    Guid UserId,
    string PersonName,
    string PhoneNumber,
    bool IsDeleted,
    string email);


public class AdminPatientResponseMapper
{
    public static readonly Expression<Func<Patient, UserDto, AdminPatientResponse>> ToPatientResponse =
        (patient, userDto) => new AdminPatientResponse(patient.Id,
            patient.UserId,
            patient.PersonName,
            patient.PhoneNumber.Value,
            patient.IsDeleted,
            userDto.Email);
}
