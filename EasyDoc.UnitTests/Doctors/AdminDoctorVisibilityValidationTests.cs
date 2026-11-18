using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Application.CQRS.Doctors.Commands;
using EasyDoc.Application.CQRS.Doctors.Commands.Admin;
using EasyDoc.SharedKernel;
using Xunit;

namespace EasyDoc.UnitTests.Doctors;

public class AdminDoctorVisibilityValidationTests
{
    [Fact]
    public async Task VisibilityOnlyUpdate_IsValid()
    {
        var validator = new UpdateDoctorCommandValidator(
            new StubPhoneNumberService(),
            dbContext: null!);

        var command = new UpdateDoctorCommand(
            Guid.NewGuid(),
            PersonName: null,
            PhoneNumber: null,
            isVisible: false,
            CityId: null,
            DefaultAppointmentTimeInMinutes: null,
            ClinicAddress: null,
            Description: Optional<string>.NotProvided,
            ProfilePictureUrl: Optional<string>.NotProvided);

        var result = await validator.ValidateAsync(command);

        Assert.True(
            result.IsValid,
            string.Join(" | ", result.Errors.Select(error => $"{error.PropertyName}: {error.ErrorMessage}")));
    }

    [Fact]
    public async Task DoctorSelfUpdate_WithoutOptionalAppointmentTime_IsValid()
    {
        var validator = new UpdateMeCommandValidator(new StubPhoneNumberService());

        var command = new UpdateMeCommand(
            PersonName: null,
            PhoneNumber: null,
            ClinicAddress: null,
            DefaultAppointmentTimeInMinutes: null,
            Description: Optional<string>.NotProvided,
            ProfilePictureUrl: Optional<string>.NotProvided);

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    private sealed class StubPhoneNumberService : IPhoneNumberService
    {
        public bool IsValid(string raw) => true;

        public bool TryNormalize(string raw, out string e164)
        {
            e164 = raw;
            return true;
        }
    }
}
