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

        var endTime = request.StartTime.AddMinutes(doctor.DefaultAppointmentTimeInMinutes); // end time is calculated dynamically

        if (endTime < request.StartTime)
            return Result.Failure<Guid>(AppointmentErrors.CrossedMidnight);

        var scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault(s => s.Date == request.Date);

        var schedule = doctor.Schedules.FirstOrDefault(s => s.DayOfWeek == request.Date.DayOfWeek);

        if (scheduleOverride is not null)
        {
            if (!scheduleOverride.IsAvailable || request.StartTime < scheduleOverride.StartTime || endTime > scheduleOverride.EndTime)
                return Result.Failure<Guid>(AppointmentErrors.DoctorNotAvailable); 
        }

        else if (schedule is not null)
        {
            if (request.StartTime < schedule.StartTime || endTime > schedule.EndTime)
                return Result.Failure<Guid>(AppointmentErrors.DoctorNotAvailable);
        }

        else
        {
            return Result.Failure<Guid>(AppointmentErrors.DoctorNotAvailable);
        }

        return await ScheduleAppointmentAsync(request.PatientId,
            request.DoctorId,
            request.Date,
            request.StartTime,
            endTime,
            cancellationToken);
    }

    private enum AppointmentDecision { Accept, Cancel }
    private async Task<Result> ResolvePendingAppoinmentAsync(Guid appointmentId, Guid doctorId, AppointmentDecision decision,
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
            appointment.Accept();
        else
            appointment.Cancel();

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
            var appointmentSpecification = new AppointmentByDoctorIdAndDateTimeSpecification(doctorId,
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