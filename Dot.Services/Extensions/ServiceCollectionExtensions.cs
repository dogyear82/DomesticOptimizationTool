using Dot.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Dot.Services.Extensions
{
    public static class ServiceCollectionExtensions
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
    }
}
