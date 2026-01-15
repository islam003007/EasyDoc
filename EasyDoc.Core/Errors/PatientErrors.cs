using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

internal static class PatientErrors
{
    public const string NotFoundCode = "Patients.NotFound";
    public static Error NotFound(Guid patientId) => Error.NotFound(NotFoundCode, $"The Patient With The ID = {patientId} was not found");
}
