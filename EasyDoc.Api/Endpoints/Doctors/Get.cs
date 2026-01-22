using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Doctors.Queries;
using EasyDoc.Domain.Entities.RefrenceData;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class Get : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("", async (IQueryHandler<GetDoctorsQuery, IReadOnlyList<GetDoctorsResponse>> handler,
            CancellationToken cancellationToken,
            Guid? cityId,
            Guid ? departmentId,
            int pageNumber = 1,
            int pageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetDoctorsQuery(cityId, departmentId, pageNumber, pageSize);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
