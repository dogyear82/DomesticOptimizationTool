using Dot.Models.Interfaces;
using Dot.Services.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Dot.Services
{
    public interface IMessageSender
    {
        Task Send<T>(IMessage<T> message);
    }

    public class MessageSender : IMessageSender
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _options;

        public MessageSender(IOptionsMonitor<RabbitMQSettings> options)
        {
            _options = options.CurrentValue;
            _connectionFactory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = "/"
            };
        }

        public async Task Send<T>(IMessage<T> message)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, true);
            await channel.QueueDeclareAsync(message.QueueName, true, false, false); 
            await channel.QueueBindAsync(message.QueueName, _options.Exchange, message.RoutingKey);

            var messageString = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageString);
            await channel.BasicPublishAsync(_options.Exchange, message.RoutingKey, body);
        }
    }
}
