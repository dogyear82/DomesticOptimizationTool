using Dot.Services.Messaging.Interfaces;

namespace Dot.ChatProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageProcessor _messageProcessor;

        public Worker(ILogger<Worker> logger, IServiceProvider sp)
        {
            _logger = logger;
            _messageProcessor = sp.GetRequiredService<IMessageProcessor>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageProcessor.Start(stoppingToken);
        }
    }
}
