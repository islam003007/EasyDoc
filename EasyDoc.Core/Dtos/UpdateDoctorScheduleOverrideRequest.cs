namespace EasyDoc.Application.Dtos;

public record UpdateDoctorScheduleOverrideRequest(Guid DoctorId, Guid ScheduleOverrideId, bool IsAvailable, TimeOnly? StartTime, TimeOnly? EndTime);
