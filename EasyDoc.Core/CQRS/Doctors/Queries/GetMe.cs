using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record GetMeQuery() : IQuery<MeDoctorResponse>;

public record MeDoctorResponse(Guid Id,
    string PersonName,
    string PhoneNumber,
    string Department,
    string City,
    string ClinicAddress,
    long DefaultAppointmentTimeInMinutes,
    string? Description,
    string? ProfilePictureUrl,
    string Email);
internal class GetMeQueryHandler : IQueryHandler<GetMeQuery, MeDoctorResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbcontext;
    private readonly IUserContext _userContext;
    public GetMeQueryHandler(IReadOnlyApplicationDbContext dbcontext, IUserContext userContext)
    {
        _dbcontext = dbcontext;
        _userContext = userContext;
    }

    public async Task<Result<MeDoctorResponse>> Handle(GetMeQuery query, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        return await _dbcontext.DoctorDetails
            .Where(doctorDetails => doctorDetails.Id == doctorId)
            .Join(_dbcontext.UserDtos,
                  doctorDetails => doctorDetails.UserId,
                  userDto => userDto.UserId,
                  (doctor, userDto) => new MeDoctorResponse(doctor.Id,
                   doctor.PersonName,
                   doctor.PhoneNumber,
                   doctor.Department,
                   doctor.City,
                   doctor.ClinicAddress,
                   doctor.DefaultAppointmentTimeInMinutes,
                   doctor.Description,
                   doctor.ProfilePictureUrl,
                   userDto.Email))
            .FirstOrDefaultAsync(cancellationToken) ?? 
            throw new CurrentUserNotFoundException(_userContext.UserId);  
    }
}
