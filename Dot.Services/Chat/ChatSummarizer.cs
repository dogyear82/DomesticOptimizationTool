using Dot.Services.Ollama;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

namespace Dot.Services.Chat
{
    public interface IChatSummarizer
    {
        public Task<string> SummarizeChatAsync(string conversationId);
        public Task<string> GenerateTitleAsync(List<ChatMessage> messages);
    }

    public class ChatSummarizer : IChatSummarizer
    {
        private readonly ILlmClientAccessor _ollamaAccessor;

        public ChatSummarizer(ILlmClientAccessor ollamaAccessor)
        {
            _ollamaAccessor = ollamaAccessor;
        }

        public async Task<string> GenerateTitleAsync(List<ChatMessage> messages)
        {
            var prompt = new ChatMessage
            {
                Role = ChatRole.User,
                Text = "Generate a title following chat. The conversation is as follows \n"
            };
            foreach(var message in messages)
            {
                prompt.Text += $"{message.Role}: {message.Text}\n";
            }
            prompt.Text += "Please answer with only 3 to 4 words and no punctuation.";

            var response = string.Empty;
            await foreach (var stream in _ollamaAccessor.ChatAsync(new List<ChatMessage> { prompt }, "mistral"))
            {
                response += stream.Text;
            }
            return Regex.Replace(response, @"^[^a-zA-Z0-9]+|[^a-zA-Z0-9]+$", "");
        }

        public Task<string> SummarizeChatAsync(string conversationId)
        {
            throw new NotImplementedException();
        }
    }
}
