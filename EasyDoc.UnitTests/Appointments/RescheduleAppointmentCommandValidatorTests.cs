using EasyDoc.Application.CQRS.Appointments.Commands;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class RescheduleAppointmentCommandValidatorTests
{
    private readonly RescheduleAppointmentCommandValidator _validator = new();

    [Fact]
    public async Task FutureDateAndTime_IsValid()
    {
        var future = DateTime.UtcNow.AddDays(7);
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(),
            DateOnly.FromDateTime(future),
            new TimeOnly(9, 30));

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid,
            string.Join(" | ", result.Errors.Select(error => error.ErrorMessage)));
    }

    [Fact]
    public async Task PastDate_IsInvalid()
    {
        var past = DateTime.UtcNow.AddDays(-1);
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(),
            DateOnly.FromDateTime(past),
            new TimeOnly(9, 30));

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            error => error.ErrorMessage.Contains("must be in the future", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task EmptyAppointmentId_IsInvalid()
    {
        var future = DateTime.UtcNow.AddDays(1);
        var command = new RescheduleAppointmentCommand(Guid.Empty,
            DateOnly.FromDateTime(future),
            new TimeOnly(9, 30));

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RescheduleAppointmentCommand.AppointmentId));
    }

    [Fact]
    public async Task MissingDateAndTime_AreInvalid()
    {
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(), default, default);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RescheduleAppointmentCommand.Date));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RescheduleAppointmentCommand.StartTime));
    }
}
