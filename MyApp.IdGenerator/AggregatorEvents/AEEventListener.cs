using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.AggregatorEvents
{
    public class AEEventListener : INotificationHandler<AEEventArgs>
    {
        readonly ILogger<AEEventListener> _logger;

        public AEEventListener(ILogger<AEEventListener> logger)
        {
            _logger = logger;
        }

        public Task Handle(AEEventArgs notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received AEEvent: {Message}", notification.Message);
            return Task.CompletedTask;
        }
    }
}
