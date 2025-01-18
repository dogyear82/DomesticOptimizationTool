using AI.Gateway.API;
using HttpUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Gateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIGateway(this IServiceCollection services)
        {
            services.AddSingleton<IHttpClientAccessor, HttpClientAccessor>();
            services.AddSingleton<ITextToSpeech, TextToSpeech>();
            services.AddSingleton<IAiGateway, AiGateway>();

            services.AddHttpClient("1MinAI", (provider, client) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["ApiUrl"]);
                client.DefaultRequestHeaders.Add("API-KEY", $"{configuration["ApiKey"]}");
            });
        }
    }
}
