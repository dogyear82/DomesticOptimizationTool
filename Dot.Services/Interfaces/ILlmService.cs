using Microsoft.Extensions.AI;

namespace Dot.Services.Interfaces
{
    public interface ILlmService
    {
        IAsyncEnumerable<ChatResponseUpdate?> ChatAsync(List<ChatMessage> conversation, string model);
    }
}
