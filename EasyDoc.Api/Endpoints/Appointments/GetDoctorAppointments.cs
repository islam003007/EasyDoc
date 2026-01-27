
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

public class GetDoctorAppointments : IEndpoint
{
    public Feature Feature => Feature.Appointments;

    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/doctor", async (IQueryHandler<GetDoctorAppointmentsQuery, IReadOnlyList<AppointmentResponse>> handler,
            CancellationToken cancellationToken,
            int PageNumber = 1,
            int PageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetDoctorAppointmentsQuery(PageNumber, PageSize);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
