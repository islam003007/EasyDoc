namespace EasyDoc.Application.Dtos;

internal record CreateAppointmentRequest(Guid PatientId, Guid DoctorId, DateOnly Date, TimeOnly StartTime);
