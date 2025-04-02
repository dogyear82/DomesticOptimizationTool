using Microsoft.Extensions.DependencyInjection;

namespace Dot.Services.Tools
{
    public static class Extensions
    {
        public static IServiceCollection AddTools(this IServiceCollection services)
        {
            var toolInterface = typeof(ITool);
            var toolTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => toolInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in toolTypes)
            {
                services.AddSingleton(typeof(ITool), type);
            }

            return services;
        }
    }
}
