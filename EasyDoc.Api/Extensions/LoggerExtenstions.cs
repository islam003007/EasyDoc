using Serilog;
using System.Diagnostics;

namespace EasyDoc.Api.Extensions;

public static class LoggerExtenstions
{
    public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration logger)
    {

        return logger
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console();
            //.WriteTo.Seq() // TODO: ADD SEQ
    }

    public static IApplicationBuilder UseCustomRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("requestPath", httpContext.Request.Path);
                diagnosticContext.Set("requestMethod", httpContext.Request.Method);
                diagnosticContext.Set("requestId", httpContext.TraceIdentifier);
                diagnosticContext.Set("traceId", Activity.Current?.TraceId);
            };
        });
    }
}
