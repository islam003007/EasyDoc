using EasyDoc.Application.CQRS.Doctors.Queries;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class SlotCalculationTests
{
    private static readonly DateOnly Day = new(2026, 11, 19);

    private static Appointment AppointmentAt(int hour, int minute, int durationMinutes = 30)
    {
        var start = new TimeOnly(hour, minute);
        return new Appointment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Day,
            start,
            start.AddMinutes(durationMinutes));
    }

    [Fact]
    public void Today_OnlyReturnsSlotsAfterTheClinicNow()
    {
        var now = new DateTime(2026, 11, 19, 9, 15, 0);

        var slots = GetDoctorAvailableSlotsQueryHandler.CalculateAvailableSlots(
            Day,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            30,
            Array.Empty<Appointment>(),
            now);

        Assert.Equal(
            new[] { new TimeOnly(9, 30), new TimeOnly(10, 0), new TimeOnly(10, 30) },
            slots.Select(slot => slot.StartTime).ToArray());
    }

    [Fact]
    public void FutureDate_ReturnsTheWholeShift()
    {
        var now = new DateTime(2026, 11, 18, 18, 0, 0);

        var slots = GetDoctorAvailableSlotsQueryHandler.CalculateAvailableSlots(
            Day,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            30,
            Array.Empty<Appointment>(),
            now);

        Assert.Equal(4, slots.Count);
        Assert.Equal(new TimeOnly(9, 0), slots[0].StartTime);
        Assert.Equal(new TimeOnly(10, 30), slots[^1].StartTime);
    }

    [Fact]
    public void BookedTime_IsRemovedAndTheNextSlotStartsAtItsEnd()
    {
        var now = new DateTime(2026, 11, 18, 18, 0, 0);
        var booked = AppointmentAt(10, 0);

        var slots = GetDoctorAvailableSlotsQueryHandler.CalculateAvailableSlots(
            Day,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            30,
            new[] { booked },
            now);

        Assert.Equal(
            new[] { new TimeOnly(9, 0), new TimeOnly(9, 30), new TimeOnly(10, 30) },
            slots.Select(slot => slot.StartTime).ToArray());
    }

    [Fact]
    public void InvalidDuration_ReturnsNoSlotsInsteadOfLoopingForever()
    {
        var slots = GetDoctorAvailableSlotsQueryHandler.CalculateAvailableSlots(
            Day,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            0,
            Array.Empty<Appointment>(),
            new DateTime(2026, 11, 18, 18, 0, 0));

        Assert.Empty(slots);
    }
}
