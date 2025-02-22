using Dot.API.Gateway.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Dot.API.Gateway
{
    public interface IGateway
    {
        INavMenu NavMenu { get; }
        IConversations Conversations { get; }
        public ILlms Llms { get; }
    }

    public class Gateway : IGateway
    {
        public INavMenu NavMenu { get; }
        public IConversations Conversations { get; }
        public ILlms Llms { get; }

        public Gateway(IServiceProvider sp)
        {
            NavMenu = sp.GetRequiredService<INavMenu>();
            Conversations = sp.GetRequiredService<IConversations>();
            Llms = sp.GetRequiredService<ILlms>();
        }
    }
}
