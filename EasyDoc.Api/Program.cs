using EasyDoc.Api;
using EasyDoc.Api.Extensions;
using EasyDoc.Application;
using EasyDoc.Infrastructure;
using EasyDoc.Infrastructure.Data.DataSeed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddWeb();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .ConfigureLogging()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

await SeederRunner.WipeDatabaseAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // TODO: swagger ui, apply migrations
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

app.UseAuthorization();

app.MapEndpoints();

app.Run();
