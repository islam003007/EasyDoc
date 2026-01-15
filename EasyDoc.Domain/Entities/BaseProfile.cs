using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities;

public abstract class BaseProfile: BaseEntity
{
    public Guid UserId { get; protected set; }
    public string PersonName { get; protected set; } = default!;
    public PhoneNumber PhoneNumber { get; protected set; } = default!; // TODO: validation
    protected BaseProfile() { }

    protected BaseProfile(Guid userId, string personName, PhoneNumber phoneNumber)
    {
        SetUserId(userId);
        SetPersonName(personName);
        SetPhoneNumber(phoneNumber);
    }

    public void SetUserId(Guid userId)
    {
        Guard.Against.Default(userId, nameof(userId));

        UserId = userId;
    }

    public void SetPersonName(string personName)
    {
        Guard.Against.NullOrEmpty(personName, nameof(personName));

        PersonName = personName.Trim();
    }

    public void SetPhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }
}
