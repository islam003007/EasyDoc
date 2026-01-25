using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EasyDoc.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DomainException domainException)
        {
            _logger.LogError(domainException, "Domain exception occurred. Code = {Code}, Extensions = {@Extensions}",
                domainException.Code,
                domainException.Extensions);
        }
        else if (exception is AppException appException)
        {
            _logger.LogError(appException, "Application exception occurred. Code = {Code}, Extensions = {@Extensions}",
                appException.Code,
                appException.Extensions);
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
        }

        httpContext.Response.StatusCode = 500;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "Server Failure",
                Detail = "An unexpected error occurred"
            }
        });
    }
}
