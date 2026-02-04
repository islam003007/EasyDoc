
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

internal class GetPatientAppointmentsForDoctor : IEndpoint
{
    public Feature Feature => Feature.Appointments;
    public bool IsAdminEndpoint => false;
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/patients/{patientId}", async (Guid patientId,
            IQueryHandler<GetPatientAppointmentsForDoctorQuery, IReadOnlyList<AppointmentResponse>> handler,
            CancellationToken cancellationToken,
            int PageNumber = 1,
            int PageSize = PageConstants.DefaultPageSize) =>
        {
            var query = new GetPatientAppointmentsForDoctorQuery(patientId, PageNumber, PageSize);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
