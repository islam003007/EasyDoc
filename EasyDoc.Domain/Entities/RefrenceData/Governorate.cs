using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities.RefrenceData;

public class Governorate : BaseEntity
{
    public string Name { get; private set; } = default!;

    private Governorate() { }

    public Governorate(string name)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
    }

}
