using Dot.Services.Interfaces;
using Microsoft.Extensions.AI;

namespace Dot.Services.Ollama
{

    public class OllamaService : ILlmService
    {
        private readonly IChatClient _client;

        public OllamaService(IChatClient client)
        {
            _client = client;
        }

        public async Task<ChatResponse> ChatAsync(List<ChatMessage> conversation, string model)
        {
            var options = new ChatOptions
            {
                ModelId = model,
            };

            return await _client.GetResponseAsync(conversation, options);
        }

        public async IAsyncEnumerable<ChatResponseUpdate?> StreamChatAsync(List<ChatMessage> conversation, string model)
        {
            var options = new ChatOptions
            {
                ModelId = model,
            };
            await foreach (var chunk in _client.GetStreamingResponseAsync(conversation, options))
            {
                yield return chunk;
            }
        }
    }
}
