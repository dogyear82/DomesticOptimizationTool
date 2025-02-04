using Dot.DataAccess.Entities;
using Dot.Models.LocalAPI;

namespace Dot.DataAccess.Repositories
{
    public interface IConversationRepository
    {
        Task<List<Conversation>> GetAllConversationsAsync();
        Task<List<ChatMessage>> GetMessagesForConversationAsync(string conversationId);
        Task<bool> AddAsync(List<ChatMessage> messages);
        Task<bool> AddAsync(List<ChatMessage> messages, string conversationId);
    }

    public class ConversationRepository : IConversationRepository
    {
        private readonly IDatabase _db;

        public ConversationRepository(IDatabase db)
        {
            _db = db;
        }

        public async Task<List<Conversation>> GetAllConversationsAsync()
        {
            var conversations = await _db.ReadAsync<Conversation>();
            return conversations;
        }

        public async Task<List<ChatMessage>> GetMessagesForConversationAsync(string conversationId)
        {
            var conversation = await _db.ReadAsync<Conversation>(conversationId);
            return conversation.Messages.OrderBy(x => x.CreatedAt).Select(message => new ChatMessage
            {
                Role = message.CreateBy,
                Content = message.Content
            }).ToList();
        }

        public async Task<bool> AddAsync(List<ChatMessage> messages)
        {
            var messagesToAdd = messages.Select(m => new Message()
            {
                CreateBy = m.Role,
                Content = m.Content
            }).ToList();

            var conversation = new Conversation()
            {
                Summary = "test summary",
                Messages = messagesToAdd

            };
            return await _db.CreateAsync(conversation);
        }

        public async Task<bool> AddAsync(List<ChatMessage> messages, string conversationId)
        {
            var messagesToAdd = messages.Select(m => new Message()
            {
                CreateBy = m.Role,
                Content = m.Content
            }).ToList();

            var conversation = await _db.ReadAsync<Conversation>(conversationId);
            conversation.Messages.AddRange(messagesToAdd);
            return await _db.UpdateAsync(conversationId, conversation);
        }
    }
}
