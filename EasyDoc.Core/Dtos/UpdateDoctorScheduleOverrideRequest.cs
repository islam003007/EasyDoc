namespace EasyDoc.Application.Dtos;

public record UpdateDoctorScheduleOverrideRequest(Guid DoctorId, Guid ScheduleOverrideId, DateOnly Date, bool IsAvailable, TimeOnly? StartTime, TimeOnly? EndTime);
