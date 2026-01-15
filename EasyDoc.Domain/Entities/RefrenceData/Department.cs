using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities.RefrenceData;

public class Department: BaseEntity
{
    public string Name { get; private set; } = default!;

    public Department(string name)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
    }

    private Department() { }
}


/*
*  Orthopedics,
Cardiology,
Surgery,
Ophthalmology,
Pediatrics,
Dentistry
*/