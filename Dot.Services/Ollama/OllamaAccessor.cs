using Dot.Models;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace Dot.Services.Ollama
{
    public interface IOllamaAccessor
    {
        IAsyncEnumerable<ChatResponseStream?> ChatAsync(List<ChatMessage> chatHistory, string model);
    }

    public class OllamaAccessor : IOllamaAccessor
    {
        private readonly IOllamaApiClient _client;

        public OllamaAccessor(IOllamaApiClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<ChatResponseStream?> ChatAsync(List<ChatMessage> chatHistory, string model)
        {
            var request = new ChatRequest
            {
                Messages = chatHistory.Select(x => new Message { Role = x.Role, Content = x.Content }),
                KeepAlive = "24h"
            };
            if (!string.IsNullOrWhiteSpace(model))
            {
                request.Model = model;
            }

            await foreach (var chunk in _client.ChatAsync(request))
            {
                yield return chunk;
            }
        }
    }
}
