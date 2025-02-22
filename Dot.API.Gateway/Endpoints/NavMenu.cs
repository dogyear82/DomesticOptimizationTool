using Dot.API.Gateway.Services;
using Dot.UI.Models;

namespace Dot.API.Gateway.Endpoints
{
    public interface INavMenu
    {
        Task<List<ConversationMenuItem>> GetConversationsAsync();
    }

    public class NavMenu : INavMenu
    {
        private readonly IHttpClientAccessor _httpClientAccessor;

        public NavMenu(IHttpClientAccessor httpClientAccessor) 
        {
            _httpClientAccessor = httpClientAccessor;
        }

        public async Task<List<ConversationMenuItem>> GetConversationsAsync()
        {
            return await _httpClientAccessor.GetAsync<List<ConversationMenuItem>>(ApiEndpoints.NavMenuConversations);
        }
    }
}
