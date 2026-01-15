namespace EasyDoc.Application.Dtos;

public record UpdatePatientRequest(Guid PatientId, string? PersonName, string? PhoneNumber);
