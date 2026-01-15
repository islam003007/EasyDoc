using Ardalis.GuardClauses;
using EasyDoc.Domain.Constants;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Exceptions;
using System.Numerics;

namespace EasyDoc.Domain.Entities.DoctorAggregate;

public class Doctor : BaseProfile, IAggregateRoot
{
    public string IdCardPictureUrl { get; private set; } = default!;
    public Guid DepartmentId { get; private set; }
    public Guid CityId { get; private set; } 
    public string ClinicAddress { get; private set; } = default!; // might change to owned type
    public string? Description { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public long DefaultAppointmentTimeInMinutes { get; private set; }
    public bool IsVisible { get; private set; } = false;
    public string NormalizedName { get; private set; } = default!;
    public string MetaphoneKeys { get; private set; } = default!;

    private List<DoctorSchedule> _schedules = new List<DoctorSchedule>();
    public IReadOnlyCollection<DoctorSchedule> Schedules => _schedules.AsReadOnly();

    private List<DoctorScheduleOverride> _scheduleOverrides = new List<DoctorScheduleOverride>();
    public IReadOnlyCollection<DoctorScheduleOverride> ScheduleOverrides => _scheduleOverrides.AsReadOnly();

    // for ef core
    private Doctor() { }

    // for using the delete method with just and id
    public Doctor(Guid id)
    {
        Id = id;
    }

    public Doctor(Guid userId,
        string personName,
        PhoneNumber phoneNumber,
        string idCardPictureUrl,
        string clinicAddress,
        Guid departmentId,
        Guid cityId,
        long defaultAppointmentTimeInMinutes = AppointmentConstants.DefaultAppointmentTimeInMinutes,
        string? description = null,
        string? profilePictureUrl = null) : base(userId, personName, phoneNumber)
    {
        SetIdCardPictureUrl(idCardPictureUrl);
        SetProfilePictureUrl(profilePictureUrl);
        SetDepartmentId(departmentId);
        SetCityId(cityId);
        SetClinicAddress(clinicAddress);
        SetDescription(description);
        SetDefaultAppointmentTimeInMinutes(defaultAppointmentTimeInMinutes);
    }
    public void SetClinicAddress(string clinicAddress)
    {
        Guard.Against.NullOrEmpty(clinicAddress);

        ClinicAddress = clinicAddress.Trim();
    }
    public void SetDescription(string? description)
    {
        if (description != null)
        {
            Guard.Against.WhiteSpace(description, nameof(description));
            description = description.Trim();
        }

        Description = description;
    }
    public void SetProfilePictureUrl(string? profilePictureUrl)
    {
        if (profilePictureUrl != null)
        {
            Guard.Against.WhiteSpace(profilePictureUrl, nameof(profilePictureUrl));
            profilePictureUrl = profilePictureUrl.Trim();
        }

        ProfilePictureUrl = profilePictureUrl;
    }

    public void SetIdCardPictureUrl(string idCardPictureUrl)
    {
        Guard.Against.NullOrEmpty(idCardPictureUrl);

        IdCardPictureUrl = idCardPictureUrl.Trim();
    }
    public void SetDefaultAppointmentTimeInMinutes(long defaultAppointmentTimeInMinutes)
    {
        Guard.Against.OutOfRange(defaultAppointmentTimeInMinutes,
            nameof(defaultAppointmentTimeInMinutes),
            AppointmentConstants.MinAppointmentTimeInMinutes,
            AppointmentConstants.MaxAppointmentTimeInMinutes);

        DefaultAppointmentTimeInMinutes = defaultAppointmentTimeInMinutes;
    }
    public void SetDepartmentId(Guid departmentId)
    {
        Guard.Against.Default(departmentId, nameof(departmentId));

        DepartmentId = departmentId;
    }
    public void SetCityId(Guid cityId)
    {
        Guard.Against.Default(cityId, nameof(cityId));

        CityId = cityId;
    }

    public void SetNormalizedName(string normalizedName)
    {
        Guard.Against.NullOrEmpty(normalizedName, nameof(NormalizedName));

        NormalizedName = normalizedName;
    }

    public void SetMetaphoneKeys(string metaphoneKeys)
    {
        Guard.Against.Empty(metaphoneKeys, nameof(metaphoneKeys));

        MetaphoneKeys = metaphoneKeys;
    }

    public void SetVisibility(bool isVisable)
    {
        IsVisible = isVisable;
    }

    public void AddSchedule(DoctorSchedule schedule)
    {
        _schedules.Add(schedule);
    }

    public void RemoveSchedule(DoctorSchedule schedule)
    {
        var isRemoved = _schedules.Remove(schedule);

        if (!isRemoved)
        {
            throw new DomainConflictException("Schedules.FailedToDelete",
                $"The schedule {schedule.Id} could not be removed because it is not associated with Doctor {this.Id}.",
                new { scheduleId = schedule.Id, DoctorId = this.Id });
        }
    }

    public void AddScheduleOverride(DoctorScheduleOverride scheduleOverride)
    {
        _scheduleOverrides.Add(scheduleOverride);
    }

    public void RemoveScheduleOverride(DoctorScheduleOverride scheduleOverride)
    {
        var isRemoved = _scheduleOverrides.Remove(scheduleOverride);

        if (!isRemoved)
        {
            throw new DomainConflictException("Schedules.FailedToDelete",
                $"the schedule override {scheduleOverride.Id} could not be removed because it is not associated with Doctor {this.Id}.",
                new {scheduleOverrideId = scheduleOverride.Id, DoctorId = this.Id});
        }
    }
}


