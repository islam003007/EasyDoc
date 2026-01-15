namespace EasyDoc.Domain.Entities.PatientAggregate;

public class Patient: BaseProfile, IAggregateRoot
{
    public bool IsDeleted { get; private set; } = false;
    private Patient() { }
    public Patient(Guid userId, string personName, PhoneNumber phoneNumber): base(userId, personName, phoneNumber)
    { 

    }
    public void SetIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }
}


