using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DomesticOptimizationTool
{
    public class Function
    {
        private readonly ILogger _logger;

        public Function(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function>();
        }

        [Function("Function")]
        public async Task RunAsync([TimerTrigger("* * * * * *")] TimerInfo myTimer)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(Environment.GetEnvironmentVariable("MessageBusConnection"));

            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                var exchangeName = "my-test-exchange"; // Topic exchange name
                var routingKey = "testkey"; // Routing key
                var queueName = "testqueue";

                var message = "An error occurred.";
                var body = Encoding.UTF8.GetBytes(message);

                await channel.ExchangeDeclareAsync(exchange: exchangeName, durable: true, type: ExchangeType.Topic);
                await channel.QueueDeclareAsync(queueName, true, false, false, null);
                await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);



                byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, world!");
                var props = new BasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = DeliveryModes.Persistent;
                await channel.BasicPublishAsync(exchangeName, routingKey, false, props, messageBodyBytes);

                Console.WriteLine($"[x] Sent '{message}' with routing key '{routingKey}'");
            }
        }
    }
}
