using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Constants;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Specifications;
using EasyDoc.Domain.Entities;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Services;

internal class DoctorsService
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    public DoctorsService(IRepository<Doctor> doctorRepository, IUserService userService, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;

    }

    public async Task<Result<Guid>> CreateDoctorAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            var result = await _userService.CreateUserAsync(request.Email, request.Password, Roles.Doctor);

            if (!result.IsSuccess)
                return result;

            Guid userId = result.Value;

            Doctor doctor = new Doctor(userId,
                request.PersonName,
                new PhoneNumber(request.PhoneNumber),
                request.IdCardPictureUrl,
                request.ClinicAddress,
                request.DepartmentId,
                request.CityId,
                request.DefaultAppointmentTimeInMinutes,
                request.Description,
                request.ProfilePictureUrl);

            await _doctorRepository.AddAsync(doctor, cancellationToken); // cancellatioToken rudandant but for consistency

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return doctor.Id;
        }
    }

    public async Task<Result> UpdateDoctorAsync(UpdateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Fix this it should fetch doctor by user id.
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId); // An alternitive would be to check if the doctor exists using anyasync(),
                                                                             // create a doctor object, attach it, update it, save.
                                                                             // this would be faster. OR: use bulk updates then check if only one
                                                                             // row was affected, but this would paypass domain logic.
        if (doctor == null)
            return Result.Failure(DoctorErrors.NotFound(request.DoctorId));


        if (request.PersonName is string personName) // just a null check
        {
            doctor.SetPersonName(personName);
        }

        if (request.PhoneNumber is string phoneNumber)
        {
            doctor.SetPhoneNumber(new PhoneNumber(phoneNumber));
        }

        if (request.isVisible is bool isVisible)
        {
            doctor.SetVisibility(isVisible);
        }

        if (request.CityId is Guid cityId)
        {
            doctor.SetCityId(cityId);
        }

        if (request.DefaultAppointmentTimeInMinutes is long defaultAppointmentTimeInMinutes)
        {
            doctor.SetDefaultAppointmentTimeInMinutes(defaultAppointmentTimeInMinutes);
        }

        if (request.ClinicAddress is string clinicAddress)
        {
            doctor.SetClinicAddress(clinicAddress);
        }

        request.Description.IfProvided(doctor.SetDescription);

        request.ProfilePictureUrl.IfProvided(doctor.SetProfilePictureUrl);

        //if (request.Description.IsProvided) // NOT A NULLABLE. treating it as a nullable will introduce bugs.
        //{
        //    doctor.UpdateDescription(request.Description.Value);
        //}

        //if (request.ProfilePictureUrl.IsProvided) // NOT A NULLABLE. treating it as a nullable will introduce bugs.
        //{
        //    doctor.UpdateProfilePictureUrl(request.ProfilePictureUrl.Value);
        //}
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();


    }
    public async Task<Result> DeleteDoctorPermanentAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            Doctor? doctorProfile = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);

            if (doctorProfile == null)
                return Result.Failure(DoctorErrors.NotFound(doctorId));

            await _doctorRepository.DeleteAsync(doctorProfile, cancellationToken);

            var result = await _userService.DeleteUserPermanentAsync(doctorProfile.UserId);

            if (!result.IsSuccess)
            {
                if (!result.IsSuccess && result.Error.Code == UserErrors.NotFoundCode)
                    throw new AppException("Users.NotFound.ByDoctor",
                        $"The user belonging to the doctor with the Id {doctorId} was not found",
                        new { PatientId = doctorId });

                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
    public async Task<Result> DeleteDoctorSoftAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            Doctor? doctorProfile = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctorProfile == null)
                return Result.Failure(DoctorErrors.NotFound(doctorId));

            doctorProfile.SetVisibility(false);

            var result = await _userService.DeleteUserSoftAsync(doctorProfile.UserId);

            if (!result.IsSuccess)
            {
                if (!result.IsSuccess && result.Error.Code == UserErrors.NotFoundCode)
                    throw new AppException("Users.NotFound.ByDoctor",
                        $"The user belonging to the doctor with the Id {doctorId} was not found",
                        new { PatientId = doctorId });

                return result;
            }


            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }

    public async Task<Result<Guid>> CreateDoctorScheduleAsync(CreateDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithSchedulesSpecification(request.DoctorId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure<Guid>(DoctorErrors.NotFound(request.DoctorId));

        if (doctor.Schedules.Any(s => s.DayOfWeek == request.DayOfWeek))
            return Result.Failure<Guid>(DoctorErrors.DuplicateSchedule(doctor.Id, request.DayOfWeek));

        var schedule = new DoctorSchedule(request.DayOfWeek, request.StartTime, request.EndTime);

        doctor.AddSchedule(schedule);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return schedule.Id; // returning the entity in both patch and updates methods might be cleaner
    }

    public async Task<Result<Guid>> CreateDoctorScheduleOverrideAsync(CreateDoctorScheduleOverrideRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithScheduleOverridesSpecification(request.DoctorId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure<Guid>(DoctorErrors.NotFound(request.DoctorId));

        if (doctor.ScheduleOverrides.Any(s => s.Date == request.Date)) // extra safety layer
            return Result.Failure<Guid>(DoctorErrors.DuplicateScheduleOverride(doctor.Id, request.Date));

        var scheduleOverride = new DoctorScheduleOverride(request.Date, request.IsAvailable, request.StartTime, request.EndTime);

        doctor.AddScheduleOverride(scheduleOverride);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return scheduleOverride.Id;
    }

    public async Task<Result> DeleteDoctorScheduleAsync(Guid doctorId, Guid scheduleId, CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithScheduleByIdSpecification(doctorId, scheduleId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound(doctorId));

        var schedule = doctor.Schedules.FirstOrDefault(s => s.Id == scheduleId); // extra safety filter because ef core has quirks with filtered includes.

        if (schedule is null)
            return Result.Failure(DoctorErrors.ScheduleNotFound(scheduleId));

        doctor.RemoveSchedule(schedule);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteDoctorScheduleOverrideAsync(Guid doctorId, Guid scheduleOverrideId, CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithScheduleOverrideByIdSpecification(doctorId, scheduleOverrideId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound(doctorId));

        var scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault(s => s.Id == scheduleOverrideId); // same extra check as before

        if (scheduleOverride is null)
            return Result.Failure(DoctorErrors.ScheduleOverrideNotFound(scheduleOverrideId));

        doctor.RemoveScheduleOverride(scheduleOverride);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> UpdateDoctorScheduleAsync(UpdateDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithScheduleByIdSpecification(request.DoctorId, request.ScheduleId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound(request.DoctorId));

        DoctorSchedule? schedule = doctor.Schedules.FirstOrDefault(s => s.Id == request.ScheduleId); // same extra check as before.

        if (schedule is null)
            return Result.Failure(DoctorErrors.ScheduleOverrideNotFound(request.ScheduleId));

        schedule.SetTime(request.StartTime, request.EndTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateDoctorScheduleOverrideAsync(UpdateDoctorScheduleOverrideRequest request, CancellationToken cancellationToken = default)
    {
        var specification = new DoctorWithScheduleOverrideByIdSpecification(request.DoctorId, request.ScheduleOverrideId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound(request.DoctorId));

        DoctorScheduleOverride? scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault(s => s.Id == request.ScheduleOverrideId); // same extra check

        if (scheduleOverride is null)
            return Result.Failure(DoctorErrors.ScheduleOverrideNotFound(request.ScheduleOverrideId));

        scheduleOverride.SetTime(request.IsAvailable, request.StartTime, request.EndTime);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


