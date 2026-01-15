namespace EasyDoc.Application.Dtos;

public record CreateDoctorRequest(string Email,
    string Password,
    string PersonName,
    string PhoneNumber,
    string IdCardPictureUrl,
    Guid DepartmentId,
    Guid CityId,
    String ClinicAddress,
    string? Description,
    string? ProfilePictureUrl,
    long DefaultAppointmentTimeInMinutes);

