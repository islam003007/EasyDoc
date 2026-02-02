using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class ExaminationSeedMaterializer : SeedMaterializerBase<Examination>
{
    public string Diagnosis { get; set; } = null!;
    public string Prescription { get; set; } = null!; 
    public string? Notes { get; set; }
    public override Examination ToDomainObject()
    {
        var examination = new Examination(Diagnosis, Prescription, Notes);

        return examination;
    }
}
