using EasyDoc.Api;
using EasyDoc.Api.Constants;
using EasyDoc.Api.Demo;
using EasyDoc.Api.Extensions;
using EasyDoc.Application;
using EasyDoc.Infrastructure;
using EasyDoc.Infrastructure.Data.DataSeed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeb(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .ConfigureLogging(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

await SeederRunner.ApplyMigrations(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "EasyDoc API V1");
        c.RoutePrefix = "swagger"; // UI at /swagger
    });
}

if (app.Configuration.GetValue<bool>("Demo:Enabled"))
{
    await SeederRunner.SeedDevelopment(app.Services);
}
else
{
    await SeederRunner.SeedProduction(app.Services);
}

app.UseCustomRequestLogging();

app.UseExceptionHandler();

app.UseCors(Policies.FrontendCors);
app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

if (app.Configuration.GetValue<bool>("Demo:Enabled"))
{
    app.MapDemoEndpoints();
}

// Dependency-free liveness probe; do not check the database here because the
// Azure SQL free-tier database must be allowed to auto-pause when idle.
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithTags("health")
    .AllowAnonymous();

app.Run();
