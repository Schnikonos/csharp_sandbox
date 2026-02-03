using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.Domain;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class SchedulerService(IServiceScopeFactory scopeFactory, ILogger<SchedulerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Perform scheduled tasks here
                logger.LogInformation("SchedulerService is running at: {0}", DateTimeOffset.Now);

                using var scope = scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<IAuthorInfoService>();
                await job.PrepareMessage(stoppingToken);

                // Wait for a specified interval before the next execution
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
