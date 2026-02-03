using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyApp.Application.service;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// add env variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        configuration.GetConnectionString("Default"),
        b => b.MigrationsAssembly("MyApp.Infrastructure")
    ));

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<IAuthorInfoService, AuthorInfoService>();
builder.Services.AddSingleton<HtmlTemplateRenderer>();
builder.Services.AddHostedService<SchedulerService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var specificOrgins = "AppOrigins";

if (builder.Environment.EnvironmentName == "Development")
{
    builder.Services.AddCors(options =>
    options.AddPolicy(name: specificOrgins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200");
        })
    );
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(specificOrgins);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
