using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.Extensions;
public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeValidPhoneNumber<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        IPhoneNumberService phoneService)
    {
        return ruleBuilder.Must(raw => raw is null || phoneService.IsValid(raw))
                          .WithMessage("{PropertyName} is not a valid phone number.");
    }

    // only refrence data is validated here. other data are validated in the services.
    public static IRuleBuilderOptions<T, Guid?> MustBeValidCityId<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IReadOnlyApplicationDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (cityId, cancellationToken) =>
        {
            if (cityId is null)
                return true;

            return await dbContext.Cities.AnyAsync(c => c.Id == cityId, cancellationToken);

        }).WithMessage("The City does not exist.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeValidCityId<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IReadOnlyApplicationDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (cityId, cancellationToken) =>
        {
            return await dbContext.Cities.AnyAsync(c => c.Id == cityId, cancellationToken);

        }).WithMessage("The City does not exist.");
    }

    public static IRuleBuilderOptions<T, Guid?> MustBeValidDepartmentId<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IReadOnlyApplicationDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (departmentId, cancellationToken) =>
        {
            if (departmentId is null)
                return true;

            return await dbContext.Departments.AnyAsync(c => c.Id == departmentId, cancellationToken);

        }).WithMessage("The Department does not exist.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeValidDepartmentId<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IReadOnlyApplicationDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (departmentId, cancellationToken) =>
        {
            return await dbContext.Departments.AnyAsync(c => c.Id == departmentId, cancellationToken);

        }).WithMessage("The Department does not exist.");
    }
}