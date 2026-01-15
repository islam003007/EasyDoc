using System.Collections.ObjectModel;

namespace EasyDoc.SharedKernel;

public record MultiError : Error
{
    public ReadOnlyCollection<Error> Errors { get; }

    public MultiError(IEnumerable<Error> errors) : this("General.MultibleErrors", "One or more errors occurred", errors)
    {

    }
    protected MultiError(string code, string description, IEnumerable<Error> errors) :
        base(code, description, ErrorType.MultiError)
    {
        if (errors.Any(e => e.Type != ErrorType.Failure))
            throw new InvalidOperationException("Multi Error type shouldn't include a failure Error type"); // to not expose an error that might
                                                                                                            // map to 500.
        Errors = errors.ToList().AsReadOnly();
    }
}
