namespace EasyDoc.Domain.Exceptions;

public class DomainConflictException : DomainException
{
    public DomainConflictException(string code, string message, object? metadata) : base(code, message, metadata)
    {
    }
}
