namespace EasyDoc.Application.Dtos;

public record CreatePatientRequest(string Email, string Password, string PersonName, string PhoneNumber);