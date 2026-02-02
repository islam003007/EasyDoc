using EasyDoc.Domain.Entities.RefrenceData;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class GovernorateSeedMaterializer : SeedMaterializerBase<Governorate>
{
    public string Name { get; set; } = null!;
    public override Governorate ToDomainObject()
    {
        var governorate = new Governorate(Name);

        SetDomainObjectId(governorate);

        return governorate;
    }
}
