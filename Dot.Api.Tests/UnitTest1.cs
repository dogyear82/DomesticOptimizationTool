using Microsoft.Extensions.AI;
using OllamaSharp;

namespace Dot.Api.Tests
{
    public class UnitTest1
    {

        [Fact]
        public async Task ProcessChunk()
        {
            IChatClient client = new OllamaApiClient("http://localhost:11434", "deepseek-r1");
            var systemPrompt = "Your name is Dot, which stands for Domestic Optimization Tool. You were created by Tan Nguyen. You are a helpful AI companion. Your speech style is casual and you answer queries directly.  You will not answer not provide more answers than is requested of you.  If you have suggestions, then ask the user if they would like to hear your suggestions.";

            var result = ChatRole.Assistant.Value.Equals("assistant");

            var message = string.Empty;
            var requests = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = ChatRole.System,
                    Text = systemPrompt
                },
                new ChatMessage
                {
                    Role = ChatRole.User,
                    Text = "why is the sky blue?"
                },
                new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Text = "The sky is blue due to an effect known as Rayleigh scattering."
                },
                new ChatMessage
                {
                    Role = ChatRole.User,
                    Text = "Interesting.  Can you tell me how that works?"
                }
            };

            var options = new ChatOptions
            {
                ModelId = "mistral",
            };

            var response = string.Empty;
            var blah = new List<ChatResponseUpdate>();
            await foreach (var chunk in client.GetStreamingResponseAsync(requests, options))
            {
                response += chunk.Text;
                blah.Add(chunk);
            }
        }
    }
}
