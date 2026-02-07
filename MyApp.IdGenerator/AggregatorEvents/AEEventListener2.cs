using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.AggregatorEvents
{
    public class AEEventListener2 : INotificationHandler<AEEventArgs>
    {
        readonly ILogger<AEEventListener2> _logger;

        public AEEventListener2(ILogger<AEEventListener2> logger)
        {
            _logger = logger;
        }

        public Task Handle(AEEventArgs notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received AEEvent2: {Message}", notification.Message);
            return Task.CompletedTask;
        }
    }
}
