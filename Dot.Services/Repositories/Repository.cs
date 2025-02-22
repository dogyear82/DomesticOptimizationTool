using Microsoft.Extensions.DependencyInjection;

namespace Dot.Services.Repositories
{
    public interface IRepository
    {
        public IConversationRepository Conversation { get; }
        public ILlmRepository Model { get; }
    }

    public class Repository : IRepository
    {
        public IConversationRepository Conversation { get; }
        public ILlmRepository Model { get; }

        public Repository(IServiceProvider sp)
        {
            Conversation = sp.GetRequiredService<IConversationRepository>();
            Model = sp.GetRequiredService<ILlmRepository>();
        }
    }
}
