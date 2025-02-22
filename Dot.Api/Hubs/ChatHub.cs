using Dot.API.Models;
using Dot.Models;
using Dot.Services;
using Dot.Services.Messaging.Interfaces;
using Dot.Services.Ollama;
using Dot.Services.Repositories;
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

        public async Task SendMessage(string content, string model, string conversationId = null)
        {
            _logger.LogInformation("Received message: {content}", content);

            try
            {
                var messages = new List<ChatMessage> { await GetSystemPrompt() };
                if (conversationId is not null)
                {
                    messages.AddRange(await GetConversationHistory(conversationId));
                    _logger.LogInformation($"Conversation history pulled for conversation ID{conversationId}");
                }

                var userMessage = CreateUserMessage(content, conversationId);
                messages.Add(userMessage);

                var responseStreams = new List<ChatResponseStream>();
                await foreach (var stream in _accessor.ChatAsync(messages, model))
                {
                    responseStreams.Add(stream);
                    await Clients.Caller.SendAsync("ReceiveMessage", "Stream", stream);
                }

                var llmResponse = new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Content = string.Join("", responseStreams.Select(x => x.Message.Content))
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
                Role = "system",
                Content = string.Join(" ", (await _appSettings.GetAsync()).SystemPrompts)
            };
        }

        private async Task<List<ChatMessage>> GetConversationHistory(string conversationId)
        {
            var conversation = await _repo.Conversation.GetConversationById(conversationId);
            return conversation.Messages
                    .OrderBy(x => x.CreatedAt)
                    .Where(x => x.Role != ChatRole.System)
                    .Select(x => new ChatMessage
                    {
                        ConversationId = conversationId,
                        Role = x.Role,
                        Content = x.Content
                    }).ToList();
        }

        private ChatMessage CreateUserMessage(string content, string conversationId)
        {
            return new ChatMessage
            {
                ConversationId = conversationId,
                Role = ChatRole.User,
                Content = content
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

                var response = new ChatResponseStream
                {
                    Message = new Message
                    {
                        Role = ChatRole.System,
                        Content = conversation.Id
                    }
                };
                await Clients.Caller.SendAsync("ReceiveMessage", "Stream", response);
            }
            else
            {
                await _repo.Conversation.UpdateAsync(messagesToAdd, conversationId);
            }
        }
    }
}
