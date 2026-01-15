namespace EasyDoc.Application.Abstractions.Exceptions;

public class TransactionRequiredException : AppException
{
    public TransactionRequiredException(): base("Transactions.Required", "This operation requires an active transaction") { }
}
