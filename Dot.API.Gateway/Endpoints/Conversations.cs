using Dot.API.Gateway.Services;
using Dot.UI.Models;

namespace Dot.API.Gateway.Endpoints
{
    public interface IConversations
    {
        Task<ConversationVm> Get(string id);
    }

    internal class Conversations : IConversations
    {
        private readonly IHttpClientAccessor _httpClientAccessor;

        public Conversations(IHttpClientAccessor httpClientAccessor)
        {
            _httpClientAccessor = httpClientAccessor;
        }

        public async Task<ConversationVm> Get(string id)
        {
            return await _httpClientAccessor.GetAsync<ConversationVm>($"{Url.Conversations}/{id}");
        }
    }
}
