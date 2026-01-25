using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;
using FluentValidation.Results;

namespace EasyDoc.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly IQueryHandler<TQuery, TResponse> _innerHandler;
        private readonly IEnumerable<IValidator<TQuery>> _validators;
        public QueryHandler(IQueryHandler<TQuery, TResponse> innerHandler, IEnumerable<IValidator<TQuery>> validators)
        {
            _innerHandler = innerHandler;
            _validators = validators;
        }
        public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            ValidationFailure[] failures = await ValidateAsync(query, _validators);

            if (failures.Length == 0)
            {
                return await _innerHandler.HandleAsync(query, cancellationToken);
            }

           return Result.Failure<TResponse>(createValidationError(failures));

        }
    }

    internal class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly ICommandHandler<TCommand, TResponse> _innerHandler;
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        public CommandHandler(ICommandHandler<TCommand, TResponse> innerHandler, IEnumerable<IValidator<TCommand>> validators)
        {
            _innerHandler = innerHandler;
            _validators = validators;
        }
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            ValidationFailure[] failures = await ValidateAsync(command, _validators);

            if (failures.Length == 0)
            {
                return await _innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure<TResponse>(createValidationError(failures));
        }
    }

    internal class CommandHandler<TCommand> : ICommandHandler<TCommand>
      where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _innerHandler;
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        public CommandHandler(ICommandHandler<TCommand> innerHandler, IEnumerable<IValidator<TCommand>> validators)
        {
            _innerHandler = innerHandler;
            _validators = validators;
        }

        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            ValidationFailure[] failures = await ValidateAsync(command, _validators);

            if (failures.Length == 0)
            {
                return await _innerHandler.HandleAsync(command, cancellationToken);
            }
            return Result.Failure(createValidationError(failures));
        }
    }

    // Validates both commands and queries
    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
       TCommand command,
       IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationError createValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => new Error(f.ErrorCode, f.ErrorMessage, ErrorType.Problem)));
}
