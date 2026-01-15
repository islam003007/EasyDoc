namespace EasyDoc.Application.Services;


internal record CompleteAppointmentRequest(Guid AppointmentId, Guid DoctorId, string Diagnosis, string Prescription, string? Notes);

