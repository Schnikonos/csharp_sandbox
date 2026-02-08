using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApp.IdGenerator.AggregatorEvents;
using MyApp.IdGenerator.ClassicEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class AsyncDemoService
    {
        private Random random = new Random();
        private ILogger<AsyncDemoService> _logger;

        public AsyncDemoService(ILogger<AsyncDemoService> logger)
        {
            _logger = logger;
        }

        public async Task RunAsyncDemo()
        {
            var tasks = Enumerable.Range(0, 10).Select(i => DemoAsync(i, "Basic"));
            var res = await Task.WhenAll(tasks);
            _logger.LogInformation("Finished ! -> {}", string.Join("\n", res));
        }

        public async Task RunAsyncDemo2()
        {
            var throttler = new SemaphoreSlim(5);
            var tasks = Enumerable.Range(0, 10).Select(async i =>
            {
                await throttler.WaitAsync();
                try
                {
                    await DemoAsync(i, "throttler");
                } 
                finally
                {
                    throttler.Release();
                }
            });
            await Task.WhenAll(tasks);
            _logger.LogInformation("Finished2 !");
        }

        private async Task<string> DemoAsync(int i, string label) 
        {
            var sleep = random.Next(10, 1000);
            await Task.Delay(sleep);
            var res = $"DEMO {i} [{label}] slept for {sleep} ms";
            _logger.LogInformation(res);
            return res;
        }
    }
}
