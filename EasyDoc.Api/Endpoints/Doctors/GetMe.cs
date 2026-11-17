using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class GetMe : IEndpoint
{
    public Feature Feature => Feature.Doctors;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/me", async (IQueryHandler<GetMeQuery, MeDoctorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetMeQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
