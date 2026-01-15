namespace EasyDoc.SharedKernel;
public readonly record struct Result
{
    private readonly Error _error;
    public bool IsSuccess { get; }
    public Error Error => _error ?? throw new InvalidOperationException(
        "Result was not properly initialized. Use Result.Success or Result.Failure.");
    private Result(bool isSuccess, Error? error = default)
    {
        if ((isSuccess && error is not null && error != Error.None) ||
            (!isSuccess && (error is null || error == Error.None)))
        {
            throw new ArgumentException("Invalid Result: Either error must null or isSuccess must be false", nameof(error));
        }

        IsSuccess = isSuccess;
        _error = error is null ? Error.None : error;
    }

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false, error);
}


public readonly record struct Result<TValue>
{
    private readonly TValue? _value; // nullable to avoid warnings if a default value is created. even tho it will throw anyway.

    private readonly Error _error;
    public bool IsSuccess { get; }
    public Error Error => _error ?? throw new InvalidOperationException(
        "Result was not properly initialized. Use Result.Success or Result.Failure.");
    internal Result(TValue? value, bool isSuccess, Error? error = default)
    {
        if ((isSuccess && error is not null && error != Error.None) ||
            (!isSuccess && (error is null || error == Error.None)))
        {
            throw new ArgumentException("Invalid Result: Either error must null or isSuccess must be false", nameof(error));
        }

        if (!isSuccess && value is not null
            || isSuccess && value is null)
        {
            throw new ArgumentException("Invalid Result: Either value must be null or isSuccess must be false", nameof(value));
        }


        _value = value;
        IsSuccess = isSuccess;
        _error = error is null ? Error.None : error;
    }

    public TValue Value => IsSuccess
    ? _value!
    : throw new InvalidOperationException("Cannot access Value of a failure result.");

    public static implicit operator Result<TValue>(TValue value) => value is not null ?
        Result.Success(value) : Result.Failure<TValue>(Error.NullValue);

    // might add implict cast from Error
}

