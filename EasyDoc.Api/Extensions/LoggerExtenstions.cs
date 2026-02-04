using Serilog;
using System.Diagnostics;

namespace EasyDoc.Api.Extensions;

public static class LoggerExtenstions
{
    //TODO: use the settings files instead of this.
    public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration logger, IConfiguration config)
    {

        return logger
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.Seq(config["Seq:Url"] ?? throw new InvalidOperationException("Seq:Url is not configured")); // TODO: ADD SEQ
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
