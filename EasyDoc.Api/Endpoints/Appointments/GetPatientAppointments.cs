using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

public class GetPatientAppointments : IEndpoint
{
    public Feature Feature => Feature.Appointments;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/patient", async (IQueryHandler<GetPatientAppointmentsQuery, IReadOnlyList<AppointmentResponse>> handler,
            CancellationToken cancellationToken,
            int PageNumber = 1,
            int PageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetPatientAppointmentsQuery(PageNumber, PageSize);

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.PatientsOnly);
    }
}
