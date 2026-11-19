using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Specifications;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.SharedKernel;
using System.Data;

namespace EasyDoc.Application.Services;

internal partial class AppointmentService
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IRepository<Appointment> appointmentRepository, IRepository<Doctor> doctorRepository, IRepository<Patient> patientRepository, IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> CompleteAppointmentAsync(CompleteAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        Appointment? appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);

        if (appointment == null)
            return Result.Failure(AppointmentErrors.NotFound(request.AppointmentId));

        if (appointment.DoctorId != request.DoctorId)
            return Result.Failure(AppointmentErrors.DoctorMismatch);

        if (appointment.Status != AppointmentStatus.Scheduled)
            return Result.Failure(AppointmentErrors.NotScheduled);

        Examination examination = new Examination(request.Diagnosis, request.Prescription, request.Notes);

        appointment.SetExamination(examination);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public Task<Result> CancelAppointentAsync(Guid doctorId, Guid appointmentId, CancellationToken cancellationToken = default)
        => ResolvePendingAppoinmentAsync(doctorId, appointmentId, AppointmentDecision.Cancel, cancellationToken);

    public async Task<Result> CancelAppointmentByPatientAsync(Guid patientId,
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        Appointment? appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null)
            return Result.Failure(AppointmentErrors.NotFound(appointmentId));

        if (appointment.PatientId != patientId)
            return Result.Failure(AppointmentErrors.PatientMismatch);

        if (!appointment.CanBeCanceledByPatient())
            return Result.Failure(AppointmentErrors.NotCancelable);

        appointment.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public Task<Result> AcceptAppointmentAsync(Guid doctorId, Guid appointmentId, CancellationToken cancellationToken = default)
        => ResolvePendingAppoinmentAsync(doctorId, appointmentId, AppointmentDecision.Accept, cancellationToken);

    public async Task<Result<Guid>> CreateAppointmentAsync(CreateAppointmentRequest request, CancellationToken cancellationToken = default)
    {                                                                                              
        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
            return Result.Failure<Guid>(PatientErrors.NotFound(request.PatientId));

        var doctorSpecification = new DoctorWithAllSchedulesSpecification(request.DoctorId);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(doctorSpecification, cancellationToken);

        if (doctor is null)
            return Result.Failure<Guid>(DoctorErrors.NotFound(request.DoctorId));

        if (!ClinicClock.IsInTheFuture(request.Date, request.StartTime))
            return Result.Failure<Guid>(AppointmentErrors.PastDate);

        var appointmentSpecification = new AppointmentByDoctorAndPatientSpecification(request.DoctorId, request.PatientId, request.Date);

        var patientAlreadyHasAppointment = await _appointmentRepository.AnyAsync(appointmentSpecification, cancellationToken);

        if (patientAlreadyHasAppointment)
            return Result.Failure<Guid>(AppointmentErrors.PatientHasAppointment);

        if (!TryResolveEndTime(doctor, request.StartTime, out var endTime))
            return Result.Failure<Guid>(AppointmentErrors.CrossedMidnight);

        if (!IsWithinAvailability(doctor, request.Date, request.StartTime, endTime))
            return Result.Failure<Guid>(AppointmentErrors.DoctorNotAvailable);

        return await ScheduleAppointmentAsync(request.PatientId,
            request.DoctorId,
            request.Date,
            request.StartTime,
            endTime,
            cancellationToken);
    }

    public async Task<Result> RescheduleAppointmentAsync(RescheduleAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        Appointment? appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound(request.AppointmentId));

        if (appointment.PatientId != request.PatientId)
            return Result.Failure(AppointmentErrors.PatientMismatch);

        if (!appointment.CanBeRescheduledByPatient())
            return Result.Failure(AppointmentErrors.NotReschedulable);

        if (!ClinicClock.IsInTheFuture(request.Date, request.StartTime))
            return Result.Failure(AppointmentErrors.PastDate);

        var duplicateSpecification = new AppointmentByDoctorAndPatientSpecification(
            appointment.DoctorId,
            appointment.PatientId,
            request.Date,
            appointment.Id);

        if (await _appointmentRepository.AnyAsync(duplicateSpecification, cancellationToken))
            return Result.Failure(AppointmentErrors.PatientHasAppointment);

        Doctor? doctor = await _doctorRepository.FirstOrDefaultAsync(
            new DoctorWithAllSchedulesSpecification(appointment.DoctorId), cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound(appointment.DoctorId));

        if (!TryResolveEndTime(doctor, request.StartTime, out var endTime))
            return Result.Failure(AppointmentErrors.CrossedMidnight);

        if (!IsWithinAvailability(doctor, request.Date, request.StartTime, endTime))
            return Result.Failure(AppointmentErrors.DoctorNotAvailable);

        // Serializable isolation level is required for scheduling. The appointment being moved is
        // excluded so rescheduling to the same time does not conflict with itself.
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken))
        {
            var appointmentSpecification = new AppointmentsOverlapSpecification(doctor.Id,
                request.Date,
                request.StartTime,
                endTime,
                request.AppointmentId);

            if (await _appointmentRepository.AnyAsync(appointmentSpecification, cancellationToken))
                return Result.Failure(AppointmentErrors.DoctorAlreadyBooked);

            appointment.Reschedule(request.Date, request.StartTime, endTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }

    internal static bool TryResolveEndTime(Doctor doctor, TimeOnly startTime, out TimeOnly endTime)
    {
        endTime = startTime.AddMinutes(doctor.DefaultAppointmentTimeInMinutes);

        return endTime >= startTime;
    }

    internal static bool IsInTheFuture(DateOnly date, TimeOnly startTime) =>
        ClinicClock.IsInTheFuture(date, startTime);

    internal static bool IsWithinAvailability(Doctor doctor, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        var scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault(s => s.Date == date);

        if (scheduleOverride is not null)
        {
            return scheduleOverride.IsAvailable
                && scheduleOverride.StartTime.HasValue
                && scheduleOverride.EndTime.HasValue
                && startTime >= scheduleOverride.StartTime.Value
                && endTime <= scheduleOverride.EndTime.Value;
        }

        var schedule = doctor.Schedules.FirstOrDefault(s => s.DayOfWeek == date.DayOfWeek);

        return schedule is not null
            && startTime >= schedule.StartTime
            && endTime <= schedule.EndTime;
    }

    private enum AppointmentDecision { Accept, Cancel }
    private async Task<Result> ResolvePendingAppoinmentAsync(Guid doctorId, Guid appointmentId, AppointmentDecision decision,
        CancellationToken cancellationToken = default)
    {
        Appointment? appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

        if (appointment == null)
            return Result.Failure(AppointmentErrors.NotFound(appointmentId));

        if (appointment.DoctorId != doctorId)
            return Result.Failure(AppointmentErrors.DoctorMismatch);

        if (appointment.Status != AppointmentStatus.Pending)
            return Result.Failure(AppointmentErrors.NotPending);

        if (decision == AppointmentDecision.Accept)
        {
            if (!ClinicClock.IsInTheFuture(appointment.Date, appointment.StartTime))
                return Result.Failure(AppointmentErrors.PastDate);

            appointment.Accept();
        }
        else
        {
            appointment.Cancel();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<Guid>> ScheduleAppointmentAsync(Guid patientId,
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken cancellationToken = default)
    {   // Serializable isolation level is required for scheduling
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)) 
        {
            var appointmentSpecification = new AppointmentsOverlapSpecification(doctorId,
                date,
                startTime,
                endTime);

            if (await _appointmentRepository.AnyAsync(appointmentSpecification, cancellationToken))
                return Result.Failure<Guid>(AppointmentErrors.DoctorAlreadyBooked);

            var appointment = new Appointment(patientId, doctorId, date, startTime, endTime);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return appointment.Id;
        }
        
    }
}