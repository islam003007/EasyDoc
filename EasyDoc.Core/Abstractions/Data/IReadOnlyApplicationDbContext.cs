using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.Domain.Entities.RefrenceData;

namespace EasyDoc.Application.Abstractions.Data;

public interface IReadOnlyApplicationDbContext
{
    public IQueryable<Appointment> Appointments { get; }
    public IQueryable<Governorate> Governorates { get; }
    public IQueryable<City> Cities { get; }
    public IQueryable<Doctor> Doctors { get; }
    public IQueryable<Department> Departments { get; }
    public IQueryable<Patient> Patients { get; }
    public IQueryable<UserDto> UserDtos { get; } // This does not represent a table.
    public IQueryable<DoctorDetailsReadModel> DoctorDetails { get; }

    // Preserve DbSet.FindAsync behavior
    Task<T?> FindAsync<T>(params object[] keyValues) where T : class;
}
