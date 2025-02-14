using Dot.Services.Ollama;
using OllamaSharp.Models.Chat;
using Dot.Models;
using System.Text.RegularExpressions;

namespace Dot.Services.Chat
{
    public interface IChatSummarizer
    {
        public Task<string> SummarizeChatAsync(string conversationId);
        public Task<string> GenerateTitleAsync(List<ChatMessage> messages);
    }

    public class ChatSummarizer : IChatSummarizer
    {
        private readonly IOllamaAccessor _ollamaAccessor;

        public ChatSummarizer(IOllamaAccessor ollamaAccessor)
        {
            _ollamaAccessor = ollamaAccessor;
        }

        public async Task<string> GenerateTitleAsync(List<ChatMessage> messages)
        {
            var prompt = new ChatMessage
            {
                Role = ChatRole.User,
                Content = "Generate a title following chat. The conversation is as follows \n"
            };
            foreach(var message in messages)
            {
                prompt.Content += $"{message.Role}: {message.Content}\n";
            }
            prompt.Content += "Please answer with only 3 to 4 words and no punctuation.";

            var response = string.Empty;
            await foreach (var stream in _ollamaAccessor.ChatAsync(new List<ChatMessage> { prompt }, "mistral"))
            {
                response += stream.Message.Content;
            }
            return Regex.Replace(response, @"^[^a-zA-Z0-9]+|[^a-zA-Z0-9]+$", "");
        }

        public Task<string> SummarizeChatAsync(string conversationId)
        {
            throw new NotImplementedException();
        }
    }
}
