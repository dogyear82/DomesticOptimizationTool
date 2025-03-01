using Microsoft.Extensions.AI;

namespace Dot.Services.Ollama
{
    public interface ILlmClientAccessor
    {
        IAsyncEnumerable<ChatResponseUpdate?> ChatAsync(List<ChatMessage> conversation, string model);
    }

    public class OllamaAccessor : ILlmClientAccessor
    {
        private readonly IChatClient _client;

        public OllamaAccessor(IChatClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<ChatResponseUpdate?> ChatAsync(List<ChatMessage> conversation, string model)
        {

            var options = new ChatOptions
            {
                ModelId = "mistral",
            };
            await foreach (var chunk in _client.GetStreamingResponseAsync(conversation, options))
            {
                yield return chunk;
            }
        }
    }
}
