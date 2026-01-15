namespace EasyDoc.Application.Abstractions.Authentication;

public interface IUserContext
{
    public Guid UserId { get; }
    public Guid DoctorId { get; }
    public Guid PatientId { get; }
}
