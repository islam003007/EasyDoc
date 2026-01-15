using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities.CityAggregate;

public class City: BaseEntity
{
    public string Name { get; private set; } = default!;
    public Guid GovernorateId { get; private set; }

    private City() { }

    public City(string name, Guid governorateId)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.Default(governorateId, nameof(governorateId));

        Name = name;
        GovernorateId = governorateId;
    }
}
