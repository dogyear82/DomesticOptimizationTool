using Dot.Services.Messaging.Interfaces;
using Dot.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Dot.Services.Messaging.Extensions
{
    public static class MessagingExtensions
    {
        public static void AddMessageSender(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory() { HostName = "localhost" };
            });
            services.AddSingleton<IMessageSender, MessageSender>();
        }

        public static void AddMessageReceiver(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory() { HostName = "localhost" };
            });
            services.AddSingleton<IMessageProcessor, MessageProcessor>();
        }

    }
}
