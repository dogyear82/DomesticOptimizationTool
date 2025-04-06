using Microsoft.Extensions.DependencyInjection;

namespace Dot.Tools
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
                if (type != typeof(ToolBase))
                {
                    services.AddSingleton(typeof(ITool), type);
                }
            }

            services.AddSingleton<IToolService, ToolService>();
            services.AddSingleton<IToolExecuter, ToolExecuter>();
            services.AddSingleton<IToolFactory, ToolFactory>();
            services.AddScoped<IAgent, Agent>();

            return services;
        }
    }
}
