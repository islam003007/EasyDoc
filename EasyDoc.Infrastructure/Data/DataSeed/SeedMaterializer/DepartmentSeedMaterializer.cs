using EasyDoc.Domain.Entities.RefrenceData;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class DepartmentSeedMaterializer : SeedMaterializerBase<Department>
{
    public string Name { get; set; } = null!;
    public override Department ToDomainObject()
    {
        var department = new Department(Name);

        SetDomainObjectId(department);

        return department;
    }
}
