using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.Domain;
using MyApp.IdGenerator.AggregatorEvents;
using MyApp.IdGenerator.ClassicEvents;
using MyApp.Infrastructure.Persistence;
using MyApp.Templating;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class SchedulerService : BackgroundService
    {
        private readonly ILogger<SchedulerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CEServiceSender _serviceSender;
        private readonly AEEventSender _aeEventSender;

        public SchedulerService(
            ILogger<SchedulerService> logger, 
            IServiceScopeFactory scopeFactory, 
            CEServiceSender serviceSender,
            AEEventSender aeEventSender
            )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _serviceSender = serviceSender;
            _aeEventSender = aeEventSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Perform scheduled tasks here
                _logger.LogInformation("SchedulerService is running at: {0}", DateTimeOffset.Now);
                _serviceSender.CreateEvent();
                await _aeEventSender.SendEvent("Test123");

                using var scope = _scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<IAuthorInfoService>();
                await job.PrepareMessage(stoppingToken);

                // Wait for a specified interval before the next execution
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
