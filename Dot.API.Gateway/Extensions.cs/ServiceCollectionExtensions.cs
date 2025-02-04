using Dot.API.Gateway.Endpoints;
using Dot.API.Gateway.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dot.API.Gateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAPIGateway(this IServiceCollection services)
        {
            services.AddSingleton<IHttpClientAccessor, HttpClientAccessor>();
            services.AddSingleton<INavMenu, NavMenu>();
            services.AddSingleton<IConversations, Conversations>();
            services.AddSingleton<IGateway, Gateway>();
            services.AddSingleton<IApiClientFactory, ApiClientFactory>();
        }
    }
}
