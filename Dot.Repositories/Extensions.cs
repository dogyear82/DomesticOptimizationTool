using Dot.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Dot.DataAccess.Extensions;

namespace Dot.Repositories
{
    public static class Extensions
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.SetupDataAccess(configuration);
            services.AddSingleton<IDatabase, MongoDatabase>();
            services.AddSingleton<ILlmRepository, LlmRepository>();
            services.AddSingleton<IConversationRepository, ConversationRepository>();
            services.AddSingleton<IRepository, Repository>();
        }
    }
}
