using EasyDoc.Domain.Entities.CityAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class CitySeedMaterializer : SeedMaterializerBase<City>
{
    public string Name { get; set; } = null!;
    public Guid GovernorateId { get; set; }
    public override City ToDomainObject()
    {
        var city = new City(Name, GovernorateId);

        SetDomainObjectId(city);

        return city;
    }
}
