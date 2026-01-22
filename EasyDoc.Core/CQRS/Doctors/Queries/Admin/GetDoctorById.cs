using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries.Admin;

public record GetDoctorByIdQuery(Guid DoctorId) : IQuery<AdminDoctorResponse>;

internal class GetMeQueryValidator : AbstractValidator<GetDoctorByIdQuery>
{
    public GetMeQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

public record AdminDoctorResponse(Guid Id,
    Guid userId,
    string PersonName,
    string PhoneNumber,
    string Department,
    string City,
    string ClinicAddress,
    string? Description,
    string? ProfilePictureUrl,
    string IdCardPictureUrl,
    long DefaultAppointmentTimeInMinutes,
    bool IsVisible,
    string Email);  // Identity

internal class GetDoctorByIdQueryHandler : IQueryHandler<GetDoctorByIdQuery, AdminDoctorResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbcontext;

    public GetDoctorByIdQueryHandler(IReadOnlyApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<Result<AdminDoctorResponse>> HandleAsync(GetDoctorByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.DoctorDetails
            .Where(doctorResponse => doctorResponse.Id == query.DoctorId)
            .Join(_dbcontext.UserDtos,
                doctorDetails => doctorDetails.UserId,
                userEmail => userEmail.UserId,
                (doctorDetails, userDto) =>
                new AdminDoctorResponse(doctorDetails.Id,
                    doctorDetails.UserId,
                    doctorDetails.PersonName,
                    doctorDetails.PhoneNumber,
                    doctorDetails.Department,
                    doctorDetails.City,
                    doctorDetails.ClinicAddress,
                    doctorDetails.Description,
                    doctorDetails.ProfilePictureUrl,
                    doctorDetails.IdCardPictureUrl,
                    doctorDetails.DefaultAppointmentTimeInMinutes,
                    doctorDetails.IsVisible,
                    userDto.Email))
            .FirstOrDefaultAsync(cancellationToken) ??
            Result.Failure<AdminDoctorResponse>(DoctorErrors.NotFound(query.DoctorId));
    }
}
