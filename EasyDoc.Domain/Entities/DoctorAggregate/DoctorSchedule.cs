using Ardalis.GuardClauses;
using EasyDoc.Domain.Gaurds;

namespace EasyDoc.Domain.Entities.DoctorAggregate;

public class DoctorSchedule: BaseEntity
{
    public DayOfWeek DayOfWeek { get; private set; } // unique per doctor
    public TimeOnly StartTime { get; private set; } // making a value object of Time intervals is way better
    public TimeOnly EndTime { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public DoctorSchedule(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        SetTime(startTime, endTime);
        DayOfWeek = dayOfWeek;  
    }
    private DoctorSchedule() { }

    public void SetTime(TimeOnly startTime, TimeOnly endTime)
    {
        Guard.Against.Default(startTime, nameof(StartTime));
        Guard.Against.Default(endTime, nameof(EndTime));
        Guard.Against.StartTimeAfterEndTime(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;
    }
}
