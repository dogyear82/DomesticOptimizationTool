using Dot.Services.Messaging.Interfaces;
using Dot.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Dot.Services.Messaging.Extensions
{
    public static class MessagingExtensions
    {
        public static void AddMessageSender(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddSingleton(GetConnectionFactory);
            services.AddSingleton<IMessageSender, MessageSender>();
        }

        public static void AddMessageReceiver(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddSingleton(GetConnectionFactory);
            services.AddSingleton<IMessageProcessor, MessageProcessor>();
        }

        private static IConnectionFactory GetConnectionFactory(IServiceProvider sp)
        {
            var rabbitSettings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            return new ConnectionFactory() 
            {
                HostName = rabbitSettings.HostName,
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = "/"
            };
        }
    }
}
