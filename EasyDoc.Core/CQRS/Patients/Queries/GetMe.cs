using EasyDoc.Application.Abstractions.Messaging;

namespace EasyDoc.Application.CQRS.Patients.Queries;

public record GetMeQuery : IQuery<PatientMeResponse>;

public record PatientMeResponse(Guid Id,
    string PersonName,
    string PhoneNumber,
    string email);
