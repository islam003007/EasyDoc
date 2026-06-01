using EasyDoc.Api;
using EasyDoc.Api.Extensions;
using EasyDoc.Application;
using EasyDoc.Infrastructure;
using EasyDoc.Infrastructure.Data.DataSeed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeb();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .ConfigureLogging(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await SeederRunner.SeedDevelopment(app.Services);

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "EasyDoc API V1");
        c.RoutePrefix = "swagger"; // UI at /swagger
    });
}

await SeederRunner.SeedProduction(app.Services);

app.UseCustomRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

app.Run();
