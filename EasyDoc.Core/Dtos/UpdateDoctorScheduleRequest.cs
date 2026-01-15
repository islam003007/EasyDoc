namespace EasyDoc.Application.Dtos;

internal record UpdateDoctorScheduleRequest(Guid DoctorId, Guid ScheduleId, TimeOnly StartTime, TimeOnly EndTime);
