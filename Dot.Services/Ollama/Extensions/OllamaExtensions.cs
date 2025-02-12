using Dot.Services.Ollama.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;

namespace Dot.Services.Ollama.Extensions
{
    public static class OllamaExtensions
    {
        public static void AddOllamaClient(this IServiceCollection services, IConfiguration config)
        {
            var options = config.GetSection("OllamaOptions").Get<OllamaOptions>();
            services.AddSingleton<IOllamaApiClient>(sp =>
            {
                return new OllamaApiClient(options.Url, options.DefaultModel);
            });
            services.AddSingleton<IOllamaAccessor, OllamaAccessor>();
        }
    }
}
