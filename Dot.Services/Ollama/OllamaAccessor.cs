using Dot.Models;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace Dot.Services.Ollama
{
    public interface IOllamaAccessor
    {
        IAsyncEnumerable<ChatResponseStream?> ChatAsync(List<ChatMessage> chatHistory);
    }

    public class OllamaAccessor : IOllamaAccessor
    {
        private readonly IOllamaApiClient _client;

        public OllamaAccessor(IOllamaApiClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<ChatResponseStream?> ChatAsync(List<ChatMessage> chatHistory)
        {
            var message = string.Empty;
            chatHistory.Add(
                new ChatMessage
                {
                    Role = ChatRole.System.ToString().ToLower(),
                    Content = "Only use the conversation history as context if it applies to the user's current query.  If the current query is unrelated then the topic has most likely changed."
                });
            var request = new ChatRequest
            {
                Model = "deepseek-r1",
                Messages = chatHistory.Select(x => new Message { Role = GetRoleFromString(x.Role), Content = x.Content })
            };

            await foreach (var chunk in _client.ChatAsync(request))
            {
                yield return chunk;
            }
        }

        private ChatRole GetRoleFromString(string role)
        {
            return role switch
            {
                "user" => ChatRole.User,
                "assistant" => ChatRole.Assistant,
                "tool" => ChatRole.Tool,
                _ => ChatRole.System
            };
        }
    }
}
