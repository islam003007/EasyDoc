namespace EasyDoc.Application.Dtos;

internal record CreateDoctorScheduleRequest(Guid DoctorId, DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
