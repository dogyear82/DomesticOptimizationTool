using Microsoft.Extensions.AI;
using Moq;

namespace Dot.Services.Ollama.Tests
{
    public class OllamaClientTests
    {
        private readonly IChatClient _client;
        private readonly IOllamaClient _ollamaClient;

        public OllamaClientTests()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/")
            };
            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var baseClient = new OllamaClient(factory.Object);
            _client = baseClient;
            _ollamaClient = baseClient;
        }

        [Fact]
        public async Task ToolResponseTest()
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant that takes a question and finds the most appropriate tool or tools to execute, along with the parameters required to run the tool  Respond as JSON using the following schema: {\"functionName\": \"function name \", \"parameters\": [{\"parameterName\": \"name of parameter\", \"parameterValue\"}].  the tools are: [{ \"name\": \"SillyTool\", \"description\": \"Generates a silly sentence using the parameters provided\", \"parameters\": [{\"name\": \"sillyString1\", \"description\": \"The first string to use to make the silly sentence.\", \"type\": \"string\", \"required\": true }, { \"name\": \"sillyString2\", \"description\": \"The second string to use to make the silly sentence.\", \"type\": \"string\", \"required\": true }]"),
                new ChatMessage(ChatRole.User, "give me something silly with the words 'stinky' and 'burger'.")
            };

            var message = string.Empty;
            await foreach(var chunk in _client.GetStreamingResponseAsync(messages))
            {
                message += chunk.Text;
            }
        }

        [Fact]
        public async Task ModelsFetchTest()
        {
            var tags = await _ollamaClient.GetTagsAsync();
            Assert.NotNull(tags);
            Assert.NotEmpty(tags.Models);
        }
    }
}
