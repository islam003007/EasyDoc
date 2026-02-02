using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class DoctorSecheduleOverrideMaterializer : SeedMaterializerBase<DoctorScheduleOverride>
{
    public DateOnly Date {  get; set; }
    public bool IsAvailable { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public override DoctorScheduleOverride ToDomainObject()
    {
        var scheduleOverride = new DoctorScheduleOverride(Date, IsAvailable, StartTime, EndTime);

        SetDomainObjectId(scheduleOverride);

        return scheduleOverride;
    }
}