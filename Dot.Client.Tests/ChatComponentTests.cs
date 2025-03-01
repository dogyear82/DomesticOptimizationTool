using Dot.UI.Models;
using Dot.Utilities.Extensions;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace Dot.Client.Tests
{
    public class ChatComponentTests
    {
        public List<ChatEntry> ChatEntries;
        public List<string> messages = new();

        public bool isThinking = false;
        public string thought = string.Empty;
        public bool isResponseFinished = true;


        [Fact]
        public void Test1()
        {
            var stringPayload = "";
        }

        [Fact]
        public async Task ProcessChunk()
        {
            IChatClient client = new OllamaApiClient("http://localhost:11434", "deepseek-r1");
            var systemPrompt = "Your name is Dot, which stands for Domestic Optimization Tool. You were created by Tan Nguyen. You are a helpful AI companion. Your speech style is casual and you answer queries directly.  You will not answer not provide more answers than is requested of you.  If you have suggestions, then ask the user if they would like to hear your suggestions.";


            var message = string.Empty;

            var request = new ChatMessage
            {
                Text = "hello",
            };

            var options = new ChatOptions
            {
                ModelId = "mistral",
            };
            await foreach (var chunk in client.GetStreamingResponseAsync(request, options))
            {
                var blah = chunk;
            }
        }
    }
}