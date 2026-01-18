using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Constants;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Domain.Entities;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Services;

internal class PatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IRepository<Patient> _patientRepository;

    public PatientService(IUnitOfWork unitOfWork, IUserService userService, IRepository<Patient> patientRepository)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> CreatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            var result = await _userService.CreateUserAsync(request.Email, request.Password, Roles.Patient);

            if (!result.IsSuccess)
                return result;

            var userId = result.Value;

            var patient = new Patient(userId, request.PersonName, new PhoneNumber(request.PhoneNumber));

            await _patientRepository.AddAsync(patient);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return patient.Id;
        }
    }

    public async Task<Result> UpdatePatientAsync(UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        Patient? patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
            return Result.Failure(PatientErrors.NotFound(request.PatientId));

        if (request.PersonName is not null)
        {
            patient.SetPersonName(request.PersonName);
        }

        if (request.PhoneNumber is not null)
        {
            patient.SetPhoneNumber(new PhoneNumber(request.PhoneNumber));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeletePatientSoftAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            Patient? patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient is null)
                return Result.Failure(PatientErrors.NotFound(patientId));

            patient.SetIsDeleted(true);

            var result = await _userService.DeleteUserSoftAsync(patient.UserId);

            if (!result.IsSuccess) 
            {
                if (!result.IsSuccess && result.Error.Code == UserErrors.NotFoundCode)
                    throw new AppException("Users.NotFound.ByPatient",
                        $"The user belonging to the Patient with the Id {patient.Id} was not found",
                        new { PatientId = patient.Id });

                return result;
            }
                

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }

    public async Task<Result> DeletePatientPermanentAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            Patient? patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient is null)
                return Result.Failure(PatientErrors.NotFound(patientId));

            await _patientRepository.DeleteAsync(patient, cancellationToken);

            var result = await _userService.DeleteUserPermanentAsync(patient.UserId);

            if (!result.IsSuccess)
            {
                if (!result.IsSuccess && result.Error.Code == UserErrors.NotFoundCode)
                    throw new AppException("Users.NotFound.ByPatient",
                        $"The user belonging to the Patient with the Id {patient.Id} was not found",
                        new { PatientId = patient.Id });

                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}