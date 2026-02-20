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
using System.Threading.RateLimiting;

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

var baseDir = AppContext.BaseDirectory;
var dbPath = Path.Combine(baseDir, "app.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        $"Data Source={dbPath}",
        b => b.MigrationsAssembly("MyApp.Infrastructure")
    ));

// register MediatR to allow application events listening and handling
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MyApp.IdGenerator.AssemblyMarker).Assembly));

// adding a rate limiter for API calls
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(1)
            }));
});

builder.Services.AddHttpClient("demoApiClient", client =>
{
    client.BaseAddress = new Uri("https://127.0.0.1:1234/");
    client.Timeout = TimeSpan.FromSeconds(10);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
    // for demo purposes only, to allow self-signed certificates when using https in the demo API
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
});


builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorInfoService, AuthorInfoService>();
builder.Services.AddSingleton<HtmlTemplateRenderer>();
//builder.Services.AddHostedService<SchedulerService>();
builder.Services.AddTransient<AsyncDemoService>();
builder.Services.AddScoped<ClientCallService>();
builder.Services.AddScoped<FileDemoService>();
builder.Services.AddScoped<SerializationDemoService>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(specificOrgins);
}

// Force-create one CEServiceListener so it subscribes to the singleton sender
app.Services.GetRequiredService<CEServiceListener>();
app.Services.GetRequiredService<CEServiceListener2>();


//app.UseHttpsRedirection();

app.UseAuthorization();

// activate rate limiting
app.UseRateLimiter();

// serve static files in wwwroot folder (e.g. for OpenAPI UI)
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
