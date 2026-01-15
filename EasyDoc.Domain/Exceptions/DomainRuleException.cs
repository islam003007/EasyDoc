namespace EasyDoc.Domain.Exceptions;

public class DomainRuleException : DomainException
{
    public DomainRuleException(string code, string message, object? metadata = null) : base(code, message, metadata)
    {
    }
}
