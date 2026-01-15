using Ardalis.GuardClauses;
using EasyDoc.Domain.Gaurds;

namespace EasyDoc.Domain.Entities.DoctorAggregate;

public class DoctorScheduleOverride: BaseEntity
{
    public DateOnly Date { get; private set; }
    public bool IsAvailable { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }
    public Doctor Doctor { get; private set; } = default!;
    public DoctorScheduleOverride(DateOnly date, bool isAvailable, TimeOnly? startTime, TimeOnly? endTime)
    {
        Guard.Against.Default(date, nameof(date));
        
        SetTime(isAvailable, startTime, endTime);
        Date = date;
    }
    private DoctorScheduleOverride() { }
    public void SetTime(bool isAvailable, TimeOnly? startTime, TimeOnly? endTime)
    {
        if (isAvailable)
        {
            Guard.Against.Null(startTime, nameof(startTime));
            Guard.Against.Null(endTime, nameof(endTime));
            Guard.Against.StartTimeAfterEndTime(startTime.Value, endTime.Value);
        }

        IsAvailable = isAvailable;
        StartTime = isAvailable ? startTime : null;
        EndTime = isAvailable ? endTime : null;
    }
}
