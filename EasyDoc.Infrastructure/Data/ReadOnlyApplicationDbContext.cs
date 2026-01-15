using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Infrastructure.Data;

internal class ReadOnlyApplicationDbContext : IReadOnlyApplicationDbContext
{
    private ApplicationDbContext _applicationDbContext;

    public ReadOnlyApplicationDbContext(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public IQueryable<Appointment> Appointments => _applicationDbContext.Appointments.AsNoTracking();
    public IQueryable<Governorate> Governorates => _applicationDbContext.Governorates.AsNoTracking();
    public IQueryable<City> Cities => _applicationDbContext.Cities.AsNoTracking();
    public IQueryable<Doctor> Doctors => _applicationDbContext.Doctors.AsNoTracking();
    public IQueryable<Department> Departments => _applicationDbContext.Departments.AsNoTracking();
    public IQueryable<Patient> Patients => _applicationDbContext.Patients.AsNoTracking();
    public IQueryable<DoctorDetailsReadModel> DoctorDetails => _applicationDbContext.DoctorDetails;
    public IQueryable<UserDto> UserDtos => _applicationDbContext.Users.Select(u => new UserDto(u.Id, u.Email!));

    // Implement FindAsync while keeping read-only semantics
    public async Task<T?> FindAsync<T>(params object[] keyValues) where T : class
    {
        var entity = await _applicationDbContext.Set<T>().FindAsync(keyValues);

        // Detach entity to enforce read-only
        if (entity != null)
            _applicationDbContext.Entry(entity).State = EntityState.Detached;

        return entity;
    }
}
