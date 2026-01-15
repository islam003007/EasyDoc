namespace EasyDoc.Application.CQRS.Doctors.Queries.Common;

// this maps to a view in the database. It can be a class instead of a record to ensure safety with ef core.
// the view can also be edited to include the doctor email. I choose to keep them separate and join them in app instead. 
public record DoctorDetailsReadModel(Guid Id,
    Guid UserId,
    string PersonName,
    string PhoneNumber,
    string Department,
    string City,
    string ClinicAddress,
    string? Description,
    string? ProfilePictureUrl,
    string IdCardPictureUrl,
    long DefaultAppointmentTimeInMinutes,
    bool IsVisible);

