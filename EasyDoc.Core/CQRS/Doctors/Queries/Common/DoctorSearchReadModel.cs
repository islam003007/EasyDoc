namespace EasyDoc.Application.CQRS.Doctors.Queries.Common;

public record DoctorSearchReadModel(
    Guid Id,
    string PersonName,
    string? ProfilePictureURL,
    string City,
    string Department,
    //  Ranking scores 
    int LexicalRank,
    int PhoneticRank)
{
    public double FinalRank => LexicalRank + (PhoneticRank * 0.5);
}
