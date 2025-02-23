using Dot.Services.Ollama.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace Dot.Services.Ollama.Extensions
{
    public static class OllamaExtensions
    {
        public static void AddOllamaClient(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("OllamaOptions").Get<OllamaOptions>();
            services.AddSingleton<IOllamaApiClient>(sp =>
            {
                var client = new OllamaApiClient(options.Url, options.DefaultModel);
                client.ChatAsync(new ChatRequest { Messages = new List<Message> { new Message { Content = "Wake up", Role = ChatRole.System } }, KeepAlive = "24h" });
                return client;
            });
            services.AddSingleton<IOllamaAccessor, OllamaAccessor>();
        }
    }
}
