using System.Diagnostics.CodeAnalysis;

namespace EasyDoc.SharedKernel;

// this type is supposed be created via the factory methods only.
public readonly record struct Result
{
    public Error Error { get; private init; }
    public bool IsSuccess { get; }
    private Result(bool isSuccess, Error? error = default)
    {
        if (isSuccess == (error is not null && error != Error.None))
        {
            throw new ArgumentException("Invalid Result: Either error must null or isSuccess must be false", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error is null ? Error.None : error;
    }

    public static Result<TValue> Success<TValue>(TValue value) => value is not null ? new(value, true) :
        throw new InvalidOperationException("Invalid Result: Success result cannot hold null");
    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);

    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false, error);
}


public readonly record struct Result<TValue>
{
    private readonly TValue _value;

    public Error Error { get; private init; }
    public bool IsSuccess { get; }
    internal Result(TValue value, bool isSuccess, Error? error = default)
    {
        if (isSuccess == (error is not null && error != Error.None))
        {
            throw new ArgumentException("Invalid Result: Either error must null or isSuccess must be false", nameof(error));
        }

        _value = value;
        IsSuccess = isSuccess;
        Error = error is null ? Error.None : error;
    }

    public TValue Value => IsSuccess
    ? _value
    : throw new InvalidOperationException("Cannot access Value of a failure result.");

    public static implicit operator Result<TValue>(TValue value) => value is not null ?
        Result.Success(value) : Result.Failure<TValue>(Error.NullValue);

    // might add implict cast from Error
}

