using EasyDoc.Domain.Entities;
using EasyDoc.Domain.Entities.PatientAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class PatientSeedMaterializer : SeedMaterializerBase<Patient>
{
    public Guid UserId { get; set; }
    public String PersonName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public override Patient ToDomainObject()
    {
        var patient = new Patient(UserId, PersonName, new PhoneNumber(PhoneNumber));

        SetDomainObjectId(patient);

        return patient;
    }
}
