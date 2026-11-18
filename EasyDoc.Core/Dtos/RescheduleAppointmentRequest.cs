namespace EasyDoc.Application.Dtos;

internal record RescheduleAppointmentRequest(Guid PatientId,
    Guid AppointmentId,
    DateOnly Date,
    TimeOnly StartTime);
