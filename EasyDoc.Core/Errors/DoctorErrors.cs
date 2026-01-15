using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

internal static class DoctorErrors // can always migrate the Error codes to enums for better comparisons.
{
    public const string NotFoundCode = "Doctors.NotFound";
    public const string ScheduleNotFoundCode = "Schedules.NotFound";
    public const string ScheduleOverrideNotFoundCode = "ScheduleOverrides.NotFound";
    public const string DuplicateScheduleCode = "Doctors.DuplicateSchedules";
    public const string DuplicateSchedulesOverrideCode = "Doctors.DuplicateScheduleOverride";
    public static Error NotFound(Guid doctorId) =>
        Error.NotFound(NotFoundCode, $"The doctor with the ID = {doctorId} was not found");
    public static Error ScheduleNotFound(Guid scheduleId) =>
        Error.NotFound(ScheduleNotFoundCode, $"The Schedule with The ID = {scheduleId} was not found for the provided doctor");
    public static Error ScheduleOverrideNotFound(Guid scheduleOverrideId) =>
        Error.NotFound(ScheduleOverrideNotFoundCode, $"The Schedule Override with The ID = {scheduleOverrideId} was not found for the provided doctor");
    public static Error DuplicateSchedule(Guid doctorId, DayOfWeek dayOfWeek) =>
        Error.Conflict(DuplicateScheduleCode, $"The doctor with the ID = {doctorId} already has a schedule with {dayOfWeek} Day of week");
    public static Error DuplicateScheduleOverride(Guid doctorId, DateOnly date) =>
        Error.Conflict(DuplicateSchedulesOverrideCode, $"The doctor with the ID = {doctorId} already has a schedule override with the date {date}");

}
