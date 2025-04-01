using Dot.Services.Interfaces;
using Dot.Services.Ollama.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dot.Services.Ollama.Extensions
{
    public static class OllamaExtensions
    {
        public static void AddOllamaClient(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("OllamaOptions").Get<OllamaOptions>();
            services.AddHttpClient("Ollama", client =>
            {
                client.BaseAddress = new Uri(options.Url);
            });
            services.AddSingleton<IChatClient, OllamaClient>();
            services.AddSingleton<ILlmService, OllamaService>();
        }
    }
}
