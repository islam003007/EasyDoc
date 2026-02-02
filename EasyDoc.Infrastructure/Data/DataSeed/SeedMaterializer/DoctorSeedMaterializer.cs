using EasyDoc.Domain.Entities;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class DoctorSeedMaterializer : SeedMaterializerBase<Doctor>
{
    public Guid UserId { get; set; }
    public string PersonName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string IdCardPictureUrl { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public Guid CityId { get;  set; }
    public string ClinicAddress { get; set; } = default!;
    public string? Description { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public long DefaultAppointmentTimeInMinutes { get; set; }
    public bool IsVisible { get; set; }
    public List<DoctorScheduleSeedMaterializer> Schedules { get; set; } = new();
    public List<DoctorScheduleOverride> ScheduleOverrides { get; set; } = new();
    public override Doctor ToDomainObject()
    {
        var doctor = new Doctor(UserId,
            PersonName,
            new PhoneNumber(PhoneNumber),
            IdCardPictureUrl,
            ClinicAddress,
            DepartmentId,
            CityId,
            DefaultAppointmentTimeInMinutes,
            Description,
            ProfilePictureUrl);

        SetDomainObjectId(doctor);

        foreach (var schedule in Schedules)
        {
            doctor.AddSchedule(schedule.ToDomainObject());
        }

        foreach (var scheduleOverride in ScheduleOverrides)
        {
            doctor.AddScheduleOverride(scheduleOverride);
        }

        if (IsVisible)
            doctor.SetVisibility(true);

        return doctor;
    }
}