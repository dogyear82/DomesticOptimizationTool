using Dot.DataAccess;
using Dot.DataAccess.Entities;
using Dot.Services.Chat;
using Microsoft.Extensions.AI;

namespace Dot.Repositories
{
    public interface IConversationRepository
    {
        Task<List<Conversation>> GetAllConversationsAsync();
        Task<Conversation> GetConversationById(string conversationId);
        Task<Conversation> AddAsync(List<ChatMessage> messages, string model);
        Task<bool> UpdateAsync(List<ChatMessage> messages, string conversationId, string model);
    }

    public class ConversationRepository : IConversationRepository
    {
        private readonly IDatabase _db;
        private readonly IChatSummarizer _summarizer;

        public ConversationRepository(IDatabase db, IChatSummarizer summarizer)
        {
            _db = db;
            _summarizer = summarizer;
        }

        public async Task<List<Conversation>> GetAllConversationsAsync()
        {
            var conversations = await _db.ReadAsync<Conversation>();
            return conversations;
        }

        public async Task<Conversation> GetConversationById(string conversationId)
        {
            return await _db.ReadAsync<Conversation>(conversationId);
        }

        public async Task<Conversation> AddAsync(List<ChatMessage> messages, string model)
        {
            var messagesToAdd = messages.Select(x => CreateMessage(x, model)).ToList();

            var conversation = new Conversation()
            {
                Title = await _summarizer.GenerateTitleAsync(messages),
                Summary = "test summary",
                Messages = messagesToAdd

            };
            return await _db.CreateAsync(conversation);
        }

        private Message CreateMessage(ChatMessage message, string model)
        {
            return new Message()
            {
                Role = message.Role.ToString(),
                Content = message.Text,
                Model = model
            };
        }

        public async Task<bool> UpdateAsync(List<ChatMessage> messages, string conversationId, string model)
        {
            var messagesToAdd = messages.Select(x => CreateMessage(x, model)).ToList();

            var conversation = await _db.ReadAsync<Conversation>(conversationId);
            conversation.Messages.AddRange(messagesToAdd);
            return await _db.UpdateAsync(conversationId, conversation);
        }
    }
}
