using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class DoctorScheduleSeedMaterializer : SeedMaterializerBase<DoctorSchedule>
{
    public DayOfWeek DayOfWeek {  get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public override DoctorSchedule ToDomainObject()
    {
        var schedule = new DoctorSchedule(DayOfWeek, StartTime, EndTime);

        SetDomainObjectId(schedule);

        return schedule;
    }
}
