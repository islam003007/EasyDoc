namespace EasyDoc.Application.Dtos;

internal record UpdateDoctorScheduleRequest(Guid DoctorId, Guid ScheduleId, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
