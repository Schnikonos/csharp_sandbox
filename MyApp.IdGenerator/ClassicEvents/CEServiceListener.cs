using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.ClassicEvents
{
    public class CEServiceListener
    {
        private readonly ILogger<CEServiceListener> _logger;

        public CEServiceListener(CEServiceSender sender, ILogger<CEServiceListener> logger)
        {
            _logger = logger;
            sender.CEEvent += HandleEvent;
        }
    
        private void HandleEvent(object? sender, CEEventArgs e)
        {
            _logger.LogInformation($"Received classic event: {e.Message}");
        }
    }
}
