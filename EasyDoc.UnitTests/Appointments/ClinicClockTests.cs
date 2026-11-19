using EasyDoc.Application.Services;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class ClinicClockTests
{
    [Fact]
    public void FutureTime_OnTheClinicClock_IsAccepted()
    {
        var now = new DateTime(2026, 11, 19, 9, 0, 0);

        Assert.True(ClinicClock.IsInTheFuture(new DateOnly(2026, 11, 19), new TimeOnly(9, 1), now));
        Assert.True(ClinicClock.IsInTheFuture(new DateOnly(2026, 11, 20), new TimeOnly(8, 0), now));
    }

    [Fact]
    public void PastTime_AndExactlyNow_AreRejected()
    {
        var now = new DateTime(2026, 11, 19, 9, 0, 0);

        Assert.False(ClinicClock.IsInTheFuture(new DateOnly(2026, 11, 19), new TimeOnly(8, 59), now));
        Assert.False(ClinicClock.IsInTheFuture(new DateOnly(2026, 11, 19), new TimeOnly(9, 0), now));
    }

    [Fact]
    public void Yesterday_IsAlwaysRejected()
    {
        var now = new DateTime(2026, 11, 19, 18, 0, 0);

        Assert.False(ClinicClock.IsInTheFuture(new DateOnly(2026, 11, 18), new TimeOnly(23, 59), now));
    }
}
