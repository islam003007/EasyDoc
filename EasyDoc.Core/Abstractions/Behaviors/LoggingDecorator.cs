using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EasyDoc.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private readonly ILogger<QueryHandler<TQuery, TResponse>> _logger;
        private readonly IQueryHandler<TQuery, TResponse>  _innerHandler;
        public QueryHandler(ILogger<QueryHandler<TQuery, TResponse>> logger, IQueryHandler<TQuery, TResponse> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default)
        {
            string queryName = query.GetType().Name;

            _logger.LogInformation("Processing Query {Query}", queryName);

            Result<TResponse> result = await _innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed query {Query}", queryName);
            }
            else
            {
                _logger.LogError("Completed query {Query} with error {@Error}", queryName, result.Error);
            }

            return result;
        }
    }

    internal class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private readonly ILogger<CommandHandler<TCommand, TResponse>> _logger;
        private readonly ICommandHandler<TCommand, TResponse> _innerHandler;

        public CommandHandler(ILogger<CommandHandler<TCommand, TResponse>> logger, ICommandHandler<TCommand, TResponse> innerHandler)
        {
         
            _logger = logger;
            _innerHandler = innerHandler;
        }

        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            string commandName = command.GetType().Name;

            _logger.LogInformation("Processing command {Command}", commandName);

            Result<TResponse> result = await _innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                _logger.LogError("Completed command {Command} with error {@Error}", commandName, result.Error);
            }

            return result;
        }
    }

    internal class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ILogger<CommandHandler<TCommand>> _logger;
        private readonly ICommandHandler<TCommand> _innerHandler;

        public CommandHandler(ILogger<CommandHandler<TCommand>> logger, ICommandHandler<TCommand> innerHandler)
        {
            _logger = logger;
            _innerHandler = innerHandler;
        }
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default)
        {
            string commandName = command.GetType().Name;

            _logger.LogInformation("Processing command {Command}", commandName);

            Result result = await _innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                _logger.LogError("Completed command {Command} with error {@Error}", commandName, result.Error);
            }

            return result; ;
        }
    }
}
