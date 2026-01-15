using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

internal class AppointmentErrors
{
    public const string DoctorNotAvailableCode = "Appointments.DoctorNotAvailable";
    public const string DoctorAlreadyBookedCode = "Appointments.DoctorAlreadyBooked";
    public const string CrossedMidnightCode = "Appointments.CrossedMidnight";
    public const string NotFoundCode = "Appointments.NotFound";
    public const string NotPendingCode = "Appointments.NotPending";
    public const string NotScheduledCode = "Appointments.NotScheduled";
    public const string DoctorMismatchCode = "Appointments.DoctorMismatch";

    public static readonly Error DoctorNotAvailable =
        Error.Problem(DoctorNotAvailableCode, "The doctor is not available for the requested date and time");
    public static readonly Error DoctorAlreadyBooked = Error.Conflict(DoctorAlreadyBookedCode, "The doctor is already booked for the selected time");
    public static readonly Error CrossedMidnight = Error.Problem(CrossedMidnightCode, "an Appointment can not cross midnight");
    public static Error NotFound(Guid appointmentId) => Error.NotFound(NotFoundCode, $"The appointment with the ID = {appointmentId} was not found");
    public static readonly Error NotPending = 
        Error.Conflict(NotPendingCode, "The requested appointment has already been accepted, completed, or canceled.");
    public static readonly Error NotScheduled =
        Error.Conflict(NotScheduledCode, "The requested appointment cannot be modified because it is either completed, canceled, or still pending.");
    public static readonly Error DoctorMismatch = Error.Problem(DoctorMismatchCode, "An appointment may only be acted upon by its owning doctor");
}
