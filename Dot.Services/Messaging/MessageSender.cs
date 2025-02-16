using Dot.Models.Interfaces;
using Dot.Services.Messaging.Interfaces;
using Dot.Services.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Dot.Services.Messaging
{
    public class MessageSender : IMessageSender
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _options;

        public MessageSender(IOptions<RabbitMQSettings> options, IConnectionFactory connectionFactory)
        {
            _options = options.Value;
            _connectionFactory = connectionFactory;
        }

        public async Task SendAsync<T>(IMessage<T> message)
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
