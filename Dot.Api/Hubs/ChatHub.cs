using Dot.API.Models;
using Dot.Models;
using Dot.Services;
using Dot.Services.Messaging.Interfaces;
using Dot.Services.Ollama;
using Dot.Services.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using Newtonsoft.Json;

namespace Dot.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILlmClientAccessor _accessor;
        private readonly IRepository _repo;
        private readonly IAppSettings<ApiSettings> _appSettings;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger, IServiceProvider sp)
        {
            _logger = logger;
            _accessor = sp.GetRequiredService<ILlmClientAccessor>();
            _repo = sp.GetRequiredService<IRepository>();
            _messageSender = sp.GetRequiredService<IMessageSender>();
            _appSettings = sp.GetRequiredService<IAppSettings<ApiSettings>>();
        }

        public async Task SendMessage(string content, string model, string conversationId = null)
        {
            _logger.LogInformation("Received message: {content}", content);

            try
            {
                var messages = new List<ChatMessage> { await GetSystemPrompt() };
                if (conversationId is not null)
                {
                    messages.AddRange(await GetConversationHistory(conversationId));
                    _logger.LogInformation($"Conversation history pulled for conversation ID {conversationId}");
                }
                
                var userMessage = CreateUserMessage(content, conversationId);
                messages.Add(userMessage);
                
                var responseStreams = new List<ChatResponseUpdate>();
                await foreach (var stream in _accessor.ChatAsync(messages, model))
                {
                    responseStreams.Add(stream);
                    var serializedStream = JsonConvert.SerializeObject(new ChatStream(stream));
                    await Clients.Caller.SendAsync("ReceiveMessage", serializedStream);
                }
                
                var llmResponse = new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Text = string.Join("", responseStreams.Select(x => x.Text))
                };
                await SaveMessages(userMessage, llmResponse, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save conversation");
            }
        }

        private async Task<ChatMessage> GetSystemPrompt()
        {
            return new ChatMessage
            {
                Role = ChatRole.System,
                Text = string.Join(" ", (await _appSettings.GetAsync()).SystemPrompts)
            };
        }

        private async Task<List<ChatMessage>> GetConversationHistory(string conversationId)
        {
            var conversation = await _repo.Conversation.GetConversationById(conversationId);
            return conversation.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.Role != ChatRole.System.ToString())
                    .Select(x => new ChatMessage
                    {
                        Role = new ChatRole(x.Role),
                        Text = x.Content
                    }).ToList();
        }

        private ChatMessage CreateUserMessage(string content, string conversationId)
        {
            return new ChatMessage
            {
                Role = ChatRole.User,
                Text = content
            };
        }

        private async Task SaveMessages(ChatMessage userMessage, ChatMessage llmResponse, string conversationId)
        {

            var messagesToAdd = new List<ChatMessage>
            {
                userMessage, llmResponse
            };
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                var conversation = await _repo.Conversation.AddAsync(messagesToAdd);
                await Clients.Caller.SendAsync("UpdateConversationId", conversation.Id);
            }
            else
            {
                await _repo.Conversation.UpdateAsync(messagesToAdd, conversationId);
            }
        }
    }
}
