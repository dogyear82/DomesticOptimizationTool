using Dot.DataAccess;
using Dot.DataAccess.Entities;
using Dot.Services.Chat;
using Microsoft.Extensions.AI;

namespace Dot.Services.Repositories
{
    public interface IConversationRepository
    {
        Task<List<Conversation>> GetAllConversationsAsync();
        Task<Conversation> GetConversationById(string conversationId);
        Task<Conversation> AddAsync(List<ChatMessage> messages);
        Task<bool> UpdateAsync(List<ChatMessage> messages, string conversationId);
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

        public async Task<Conversation> AddAsync(List<ChatMessage> messages)
        {
            var messagesToAdd = messages.Select(m => new Message()
            {
                Role = m.Role.ToString(),
                Content = m.Text
            }).ToList();

            var conversation = new Conversation()
            {
                Title = await _summarizer.GenerateTitleAsync(messages),
                Summary = "test summary",
                Messages = messagesToAdd

            };
            return await _db.CreateAsync(conversation);
        }

        public async Task<bool> UpdateAsync(List<ChatMessage> messages, string conversationId)
        {
            var messagesToAdd = messages.Select(m => new Message()
            {
                Role = m.Role.ToString(),
                Content = m.Text
            }).ToList();

            var conversation = await _db.ReadAsync<Conversation>(conversationId);
            conversation.Messages.AddRange(messagesToAdd);
            return await _db.UpdateAsync(conversationId, conversation);
        }
    }
}
