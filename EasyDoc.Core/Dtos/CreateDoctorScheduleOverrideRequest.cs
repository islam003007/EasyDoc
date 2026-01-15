namespace EasyDoc.Application.Dtos;

internal record CreateDoctorScheduleOverrideRequest(Guid DoctorId, DateOnly Date, bool IsAvailable, TimeOnly? StartTime, TimeOnly? EndTime);
