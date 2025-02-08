using Dot.Models;
using Dot.UI.Models;
using Dot.Utilities.Extensions;

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

        private void ProcessChunk(LlmResponseChunk chunk)
        {
            if (chunk.IsBeginningOfThought())
            {
                isThinking = true;
            }
            else if (chunk.IsEndOfThought())
            {
                isThinking = false;
            }
            else if (isThinking)
            {
                thought += chunk.Message.Content;
            }
            else if (chunk.Done)
            {
                var aiResponse = "";
                foreach (var message in messages)
                {
                    aiResponse += message;
                }
                messages.Clear();
                var chatEntry = new ChatEntry
                {
                    Index = 1,
                    IsUser = false,
                    Content = aiResponse,
                    Thought = thought
                };
                ChatEntries.Add(chatEntry);
                thought = string.Empty;
            }
            else
            {
                messages.Add(chunk.Message.Content);
                isResponseFinished = chunk.Done;
            }
        }
    }
}