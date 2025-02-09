using Dot.Services.Messaging.Interfaces;
using Dot.Services.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dot.Services.Messaging
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _options;
        private readonly IMessageHandler _messageHandler;

        public MessageProcessor(IOptionsMonitor<RabbitMQSettings> options, IServiceProvider sp)
        {
            _options = options.CurrentValue;
            _connectionFactory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = "/"
            };
            _messageHandler = sp.GetRequiredService<IMessageHandler>();
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += _messageHandler.HandleAsync;

            await channel.BasicConsumeAsync("inboundchat", autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
