using HttpUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Dot.Tools
{
    public static class Extensions
    {
        public static IServiceCollection AddTools(this IServiceCollection services, Action<HttpClient> client)
        {
            var toolInterface = typeof(ITool);
            var toolTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => toolInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in toolTypes)
            {
                if (type != typeof(ToolBase))
                {
                    services.AddSingleton(typeof(ITool), type);
                }
            }

            services.AddSingleton<IHttpClientAccessor, HttpClientAccessor>();
            services.AddSingleton<IToolService, ToolService>();
            services.AddSingleton<IToolExecuter, ToolExecuter>();
            services.AddSingleton<IToolFactory, ToolFactory>();
            services.AddSingleton<IAgent, Agent>();
            services.AddSingleton<IAgent, CoHost>();
            services.AddSingleton<IAgentProvider, AgentProvider>();

            services.AddHttpClient("ToolWebClient", client)
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
                });

            return services;
        }
    }
}
