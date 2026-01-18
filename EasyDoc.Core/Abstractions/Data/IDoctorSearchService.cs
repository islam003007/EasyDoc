using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Abstractions.Data;

public interface IDoctorSearchService
{
    Task<Result<IReadOnlyList<DoctorSearchReadModel>>> SearchDoctorsAsync(string SearchInput,
        Guid? cityId = null,
        Guid? departmentId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}