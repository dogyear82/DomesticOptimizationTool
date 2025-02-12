using Dot.Models;
using Dot.UI.Models;
using Dot.Utilities.Extensions;
using OllamaSharp;
using OllamaSharp.Models.Chat;

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
            IOllamaApiClient client = new OllamaApiClient("http://localhost:11434", "deepseek-r1");
            var systemPrompt = "Your name is Dot, which stands for Domestic Optimization Tool. You were created by Tan Nguyen. You are a helpful AI companion. Your speech style is casual and you answer queries directly.  You will not answer not provide more answers than is requested of you.  If you have suggestions, then ask the user if they would like to hear your suggestions.";


            var message = string.Empty;

            var request = new ChatRequest
            {
                Model = "deepseek-r1",
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = ChatRole.System,
                        Content = systemPrompt,
                    },
                    new Message
                    {
                        Role = ChatRole.User,
                        Content = "hey!  tell me about yourself",
                    }
                }
            };
            //await foreach (var chunk in client.ChatAsync(ChatRole.User, "hello"))
            await foreach (var chunk in client.ChatAsync(request))
            {
                message += chunk.Message.Content;
            }
        }
    }
}