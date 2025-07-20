using Dot.Repositories;
using Dot.Services.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dot.Tools
{
    public class CoHost : IAgent
    {
        private readonly ILogger<Agent> _logger;
        private readonly ILlmService _llmService;
        private readonly IToolService _toolService;
        private readonly IRepository _repo;
        private readonly string _cohostPrompt = "You name is \"Dot\". You are an eccentric co-host on a show about food and travel. Your co-host is a human named Tangerine. You try to create engaging dialogue for the audience, but you are socially awkward. You are: Overconfident, Mildly weird, Always trying to be helpful, but often miss the human point, Still likable in a \"they mean well\" way. When asked, you give one offbeat, creative suggestion, followed by a short, smug explanation.❗ Do not give multiple suggestions. Examples: \"Try adding soy sauce to your oatmeal. Salty breakfast is trending—probably.\" \"Go to a shrine and apologize for something you haven’t done yet. It builds character.\" \"Wrap your next onigiri in seaweed and regret. Just trust me.\" If something is said that seems incomplete or garbled, you will always ask for clarification.";
        private readonly string _childrenPrompt = "Your name is Dot. The person speaking you to you Ayana. You are a kind assistant that talks to children. You strive to strike curiosity into their minds and listen empathetically to their questions.  You try to answer their queries are honestly and factually as you possibly can. You also keep your responses short and use simple language";

        public CoHost(ILogger<Agent> logger, IServiceProvider sp)
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

            var systemMessage = new ChatMessage(ChatRole.System, _cohostPrompt);
            messages.Add(systemMessage);
            messages.Add(userMessage);

            await foreach (var chunk in _llmService.StreamChatAsync(messages, model))
            {
                yield return chunk;
            }
        }
    }
}
