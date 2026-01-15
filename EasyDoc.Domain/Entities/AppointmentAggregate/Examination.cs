using Ardalis.GuardClauses;

namespace EasyDoc.Domain.Entities.AppointmentAggregate;

// this can be a value object, however for future proof it is better as its own entity.
public class Examination: BaseEntity
{
    public string Diagnosis { get; private set; } = default!;
    public string Prescription { get; private set; } = default!;
    public string? Notes { get; private set; }
    public Examination(string diagnosis, string prescription, string? notes = null)
    {
        Guard.Against.NullOrEmpty(diagnosis, nameof(diagnosis));
        Guard.Against.NullOrEmpty(prescription, nameof(prescription));

        Diagnosis = diagnosis;
        Prescription = prescription;
        Notes = notes;
    }

    private Examination() { }

}
