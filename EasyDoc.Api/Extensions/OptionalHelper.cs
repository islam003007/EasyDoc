using EasyDoc.SharedKernel;

namespace EasyDoc.Api.Extensions;

public static class OptionalHelper
{
    public static Result<Optional<T>> TryCreate<T>(T? value, bool? clearFlag, string fieldName)
    {
        if (value is not null && clearFlag == true)
        {
            return Result.Failure<Optional<T>>(Error.Problem("HTTP.Patch",
                $"{fieldName} and Clear{fieldName} shouldn't both be set at the same Time"));
        }

        if (clearFlag is true)
            return Optional<T>.NullValue;

        if (value is not null)
            return new Optional<T>(value);

        return Optional<T>.NotProvided();
    }
}
