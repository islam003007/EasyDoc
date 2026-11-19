using EasyDoc.Application.CQRS.Appointments.Queries;
using EasyDoc.Application.Constants;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class AppointmentQueryValidatorTests
{
    [Fact]
    public void HistoryQuery_RequiresAPositivePageNumber()
    {
        var validator = new GetPatientAppointmentsForDoctorQueryValidator();
        var query = new GetPatientAppointmentsForDoctorQuery(Guid.NewGuid(), PageNumber: 0);

        var result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(query.PageNumber));
    }

    [Fact]
    public void HistoryQuery_RejectsAPageSizeAboveTheMaximum()
    {
        var validator = new GetPatientAppointmentsForDoctorQueryValidator();
        var query = new GetPatientAppointmentsForDoctorQuery(
            Guid.NewGuid(),
            PageNumber: 1,
            PageSize: PageConstants.MaxPageSize + 1);

        var result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(query.PageSize));
    }

    [Fact]
    public void PatientAndDoctorAppointmentQueries_RequirePositivePageNumbers()
    {
        var patientResult = new GetPatientAppointmentsQueryValidator()
            .Validate(new GetPatientAppointmentsQuery(PageNumber: 0));
        var doctorResult = new GetDoctorAppointmentsQueryValidator()
            .Validate(new GetDoctorAppointmentsQuery(PageNumber: 0));

        Assert.False(patientResult.IsValid);
        Assert.False(doctorResult.IsValid);
    }
}
