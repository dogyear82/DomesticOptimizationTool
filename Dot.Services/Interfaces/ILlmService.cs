using Microsoft.Extensions.AI;

namespace Dot.Services.Interfaces
{
    public interface ILlmService
    {
        Task<ChatResponse> ChatAsync(List<ChatMessage> conversation, string model);
        IAsyncEnumerable<ChatResponseUpdate?> StreamChatAsync(List<ChatMessage> conversation, string model);
    }
}
