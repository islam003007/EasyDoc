namespace EasyDoc.Api.Endpoints;

public enum Feature
{
    Auth,
    Users,
    Patients,
    Doctors,
    Appointments
}

public interface IEndpoint
{
    public Feature Feature { get; }
    public bool IsAdminEndpoint { get; }
    // The default implementation is mostly fine. some cases would require explicit names to avoid ambiguity.
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app);
}
