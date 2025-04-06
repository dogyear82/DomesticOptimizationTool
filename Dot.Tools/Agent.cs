using Dot.Repositories;
using Dot.Services.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dot.Tools
{
    public interface IAgent
    {
        IAsyncEnumerable<ChatResponseUpdate> ChatAsync(ChatMessage userMessage, string model, string conversationId);
    }

    public class Agent : IAgent
    {
        private readonly ILogger<Agent> _logger;
        private readonly ILlmService _llmService;
        private readonly IToolService _toolService;
        private readonly IRepository _repo;

        public Agent(ILogger<Agent> logger, IServiceProvider sp)
        {
            _logger = logger;
            _llmService = sp.GetRequiredService<ILlmService>();
            _repo = sp.GetRequiredService<IRepository>();
            _toolService = sp.GetRequiredService<IToolService>();
        }

        public async IAsyncEnumerable<ChatResponseUpdate> ChatAsync(ChatMessage userMessage, string model, string conversationId)
        {
            _logger.LogInformation($"Agent received message for {model}: {userMessage.Text}");
            var messages = new List<ChatMessage>();
            if (conversationId is not null)
            {
                messages.AddRange(await _repo.Conversation.GetConversationHistoryAsync(conversationId));
                _logger.LogError($"Conversation history pulled for conversation ID {conversationId}");
            }

            messages.Add(userMessage);

            var toolResponse = await GetToolResponseAsync(userMessage);
            var toolResult = await _toolService.Process(toolResponse);
            _logger.LogInformation($"Received response from tool: {toolResult}");
            var systemMessage = new ChatMessage(ChatRole.System, toolResult);
            messages.Add(systemMessage);

            await foreach (var chunk in _llmService.StreamChatAsync(messages, model))
            {
                yield return chunk;
            }
        }

        private async Task<string> GetToolResponseAsync(ChatMessage message)
        {
            _logger.LogInformation($"Determining which tool to use based on the conversation history");
            var prompts = new List<ChatMessage>();
            prompts.Add(new ChatMessage(ChatRole.System, _toolService.GetToolPrompt()));
            prompts.Add(message);
            var response = await _llmService.ChatAsync(prompts, "mistral");
            _logger.LogInformation($"Tool response received: {response.Text}");
            return response.Text;
        }
    }
}
