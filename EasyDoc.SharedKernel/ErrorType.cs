namespace EasyDoc.SharedKernel;

public enum ErrorType
{
    Failure = 0, // maps to a generic 500 error.
    MultiError, // a collection of errors
    Conflict,
    Problem, // maps to a generic 400 error.
    NotFound,
}
