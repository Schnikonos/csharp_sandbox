using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class FileDemoService
    {
        private readonly ILogger<FileDemoService> _logger;

        public FileDemoService(ILogger<FileDemoService> logger)
        {
            _logger = logger;
        }

        public void DemoFile()
        {
            File.WriteAllText("myFile.txt", $"This is a new File\nDate: {new DateTime()}");
            string content = File.ReadAllText("myFile.txt");
            _logger.LogInformation(content);
        }
    }
}
