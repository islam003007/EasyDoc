namespace EasyDoc.Api;

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
    public void MapEndpoint(IEndpointRouteBuilder app);
}
