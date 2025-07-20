using Dot.Models;
using Dot.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using Dot.Tools;

namespace Dot.API.Hubs
{
    public class CoHostHub : Hub
    {
        private readonly IAgent cohost;
        private readonly IRepository _repo;
        private readonly ILogger<ChatHub> _logger;

        public CoHostHub(ILogger<ChatHub> logger, IServiceProvider sp)
        {
            _logger = logger;
            cohost = sp.GetRequiredService<IAgentProvider>().GetAgent<CoHost>();
            _repo = sp.GetRequiredService<IRepository>();
        }

        public async Task SendMessage(string content, string model, string conversationId = null)
        {
            _logger.LogInformation("Received message for {model}: {content}", model, content);

            try
            {
                var userMessage = new ChatMessage(ChatRole.User, content);
                var responseStreams = new List<ChatResponseUpdate>();
                await foreach (var stream in cohost.ChatAsync(userMessage, model, conversationId))
                {
                    responseStreams.Add(stream);
                    await Clients.Caller.SendAsync("ReceiveMessage", new ChatStream(stream));
                }
                
                var llmResponse = new ChatMessage(ChatRole.Assistant, string.Join("", responseStreams.Select(x => x.Text)));
                await SaveMessages(userMessage, llmResponse, conversationId, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save conversation");
            }
        }

        private async Task SaveMessages(ChatMessage userMessage, ChatMessage llmResponse, string conversationId, string model)
        {
            var messagesToAdd = new List<ChatMessage>
            {
                userMessage, llmResponse
            };
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                var conversation = await _repo.Conversation.AddAsync(messagesToAdd, model);
                await Clients.Caller.SendAsync("UpdateConversationId", conversation.Id);
            }
            else
            {
                await _repo.Conversation.UpdateAsync(messagesToAdd, conversationId, model);
            }
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
    }
}
