using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.ClassicEvents
{
    public class CEServiceListener2
    {
        private readonly ILogger<CEServiceListener2> _logger;

        public CEServiceListener2(CEServiceSender sender, ILogger<CEServiceListener2> logger)
        {
            _logger = logger;
            sender.CEEvent += HandleEvent;
        }

        private void HandleEvent(object? sender, CEEventArgs e)
        {
            _logger.LogInformation($"Received classic event2: {e.Message}");
        }
    }
}
