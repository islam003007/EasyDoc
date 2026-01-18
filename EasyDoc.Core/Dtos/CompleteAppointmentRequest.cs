namespace EasyDoc.Application.Services;


internal record CompleteAppointmentRequest(Guid DoctorId, Guid AppointmentId, string Diagnosis, string Prescription, string? Notes);

