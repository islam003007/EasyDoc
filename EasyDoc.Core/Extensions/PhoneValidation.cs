using EasyDoc.Application.Abstractions.Utils;
using FluentValidation;

public static class PhoneValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeValidPhoneNumber<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        IPhoneNumberService phoneService)
    {
        return ruleBuilder.Must(raw => raw is null || phoneService.IsValid(raw))
                          .WithMessage("{PropertyName} is not a valid phone number.");
    }
}