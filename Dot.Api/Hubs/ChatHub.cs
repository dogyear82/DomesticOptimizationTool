using Dot.API.Models;
using Dot.DataAccess.Repositories;
using Dot.Models;
using Dot.Models.Messaging;
using Dot.Services;
using Dot.Services.Messaging.Interfaces;
using Dot.Services.Ollama;
using Microsoft.AspNetCore.SignalR;
using OllamaSharp.Models.Chat;

namespace Dot.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IOllamaAccessor _accessor;
        private readonly IRepository _repo;
        private readonly IAppSettings<ApiSettings> _appSettings;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger, IServiceProvider sp)
        {
            _logger = logger;
            _accessor = sp.GetRequiredService<IOllamaAccessor>();
            _repo = sp.GetRequiredService<IRepository>();
            _messageSender = sp.GetRequiredService<IMessageSender>();
            _appSettings = sp.GetRequiredService<IAppSettings<ApiSettings>>();
        }

        public async Task SendMessage(string content, string conversationId = null)
        {
            _logger.LogDebug("Received message: {content}", content);

            var messages = new List<ChatMessage> { await GetSystemPrompt() };
            if (conversationId is not null)
            {
                messages.AddRange(await GetConversationHistory(conversationId));
            }

            var userMessage = CreateUserMessage(content, conversationId);
            messages.Add(userMessage);

            var responseStreams = new List<ChatResponseStream>();
            await foreach (var stream in _accessor.ChatAsync(messages))
            {
                responseStreams.Add(stream);
                await Clients.Caller.SendAsync("ReceiveMessage", "Stream", stream);
            }

            var llmResponse = new ChatMessage
            {
                ConversationId = conversationId,
                Role = Role.Assistant,
                Content = string.Join("", responseStreams.Select(x => x.Message.Content))
            };
            messages.Add(llmResponse);

            try
            {
                await SendMessage(userMessage);
                await SendMessage(llmResponse);
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
                Role = "system",
                Content = string.Join(" ", (await _appSettings.GetAsync()).SystemPrompts)
            };
        }

        private async Task<List<ChatMessage>> GetConversationHistory(string conversationId)
        {
            var conversation = await _repo.Conversation.GetConversationById(conversationId);
            return conversation.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.CreatedBy != "system")
                    .Select(x => new ChatMessage
                    {
                        ConversationId = conversationId,
                        Role = x.CreatedBy,
                        Content = x.Content
                    }).ToList();
        }

        private ChatMessage CreateUserMessage(string content, string conversationId)
        {
            return new ChatMessage
            {
                ConversationId = conversationId,
                Role = Role.User,
                Content = content
            };
        }

        private async Task SendMessage(ChatMessage message)
        {
            var chatMessage = new InboundChatMessage(message);
            await _messageSender.Send(chatMessage);
        }
    }
}
