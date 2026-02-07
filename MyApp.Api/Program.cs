using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyApp.Application.service;
using MyApp.IdGenerator.AggregatorEvents;
using MyApp.IdGenerator.ClassicEvents;
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

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseSqlite(
        configuration.GetConnectionString("Default"),
        b => b.MigrationsAssembly("MyApp.Infrastructure")
    ));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MyApp.IdGenerator.AssemblyMarker).Assembly));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorInfoService, AuthorInfoService>();
builder.Services.AddSingleton<HtmlTemplateRenderer>();
builder.Services.AddHostedService<SchedulerService>();

builder.Services.AddSingleton<CEServiceSender>();
builder.Services.AddTransient<CEServiceListener>();
builder.Services.AddTransient<CEServiceListener2>();

builder.Services.AddSingleton<AEEventSender>();
builder.Services.AddTransient<AEEventListener>();
builder.Services.AddTransient<AEEventListener2>();

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

// Force-create one CEServiceListener so it subscribes to the singleton sender
app.Services.GetRequiredService<CEServiceListener>();
app.Services.GetRequiredService<CEServiceListener2>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
