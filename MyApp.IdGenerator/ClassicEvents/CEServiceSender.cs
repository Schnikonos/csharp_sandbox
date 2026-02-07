using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.ClassicEvents
{
    public class CEServiceSender(ILogger<CEServiceSender> logger)
    {
        public event EventHandler<CEEventArgs>? CEEvent;

        public void CreateEvent()
        {
            logger.LogInformation("Creating classic event...");
            CEEvent?.Invoke(this, new CEEventArgs { Message = "Test" });
        }
    }
}
