using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Doctors.Queries.Admin;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Admin;

internal class Get : IEndpoint
{
    public Feature Feature => Feature.Doctors;
    public bool IsAdminEndpoint => true;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("", async (IQueryHandler<GetDoctorsQuery, IReadOnlyList<AdminDoctorResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetDoctorsQuery(pageNumber, pageSize);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Policies.AdminOnly);
    }
}
