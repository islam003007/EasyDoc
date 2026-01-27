using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Patients.Queries.Admin;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients.Admin;

public class Get : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => true;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("", async (IQueryHandler<GetPatientsQuery, IReadOnlyList<AdminPatientResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetPatientsQuery(pageNumber, pageSize);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
