using EasyDoc.Domain.Entities.DoctorAggregate;
using Xunit;

namespace EasyDoc.UnitTests.Doctors;

public class DoctorScheduleEditingTests
{
    [Fact]
    public void Schedule_CanMoveToAnotherDay()
    {
        var schedule = new DoctorSchedule(
            DayOfWeek.Monday,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0));

        schedule.SetDayOfWeek(DayOfWeek.Thursday);

        Assert.Equal(DayOfWeek.Thursday, schedule.DayOfWeek);
        Assert.Equal(new TimeOnly(9, 0), schedule.StartTime);
        Assert.Equal(new TimeOnly(17, 0), schedule.EndTime);
    }

    [Fact]
    public void Override_CanMoveToAnotherDate()
    {
        var original = new DateOnly(2026, 11, 19);
        var moved = new DateOnly(2026, 11, 26);
        var scheduleOverride = new DoctorScheduleOverride(
            original,
            isAvailable: false,
            startTime: null,
            endTime: null);

        scheduleOverride.SetDate(moved);

        Assert.Equal(moved, scheduleOverride.Date);
        Assert.False(scheduleOverride.IsAvailable);
    }
}
