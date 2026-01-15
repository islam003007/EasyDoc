
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Doctors.Queries;
using System.Threading;
using Web.Api.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EasyDoc.Api.Endpoints.Doctors;

public class Search : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/search", async (IQueryHandler<SearchDoctorsQuery, IReadOnlyList<SearchDoctorResponse>> handler,
            CancellationToken cancellationToken,
            string query,
            Guid? cityId = null,
            Guid? departmentId = null,
            int pageNumber = 1,
            int pageSize = PageConstants.DefaultPageSize) =>
        {
            var searchQuery = new SearchDoctorsQuery(query, cityId, departmentId, pageNumber, pageSize);

            var result = await handler.Handle(searchQuery, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
