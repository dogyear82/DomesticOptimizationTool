using Dot.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Dot.DataAccess.Extensions;

namespace Dot.Services.Repositories
{
    public static class Extensions
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.SetupDataAccess(configuration);
            services.AddSingleton<IDatabase, MongoDatabase>();
            services.AddScoped<ILlmRepository, LlmRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IRepository, Repository>();
        }
    }
}
