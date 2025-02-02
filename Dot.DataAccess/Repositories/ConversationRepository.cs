using Dot.DataAccess.Entities;
using Dot.Models.LocalAPI;

namespace Dot.DataAccess.Repositories
{
    public interface IConversationRepository
    {
        Task<LocalChatMessage> GetAsync(string conversationId);
        Task<bool> AddAsync(List<LocalChatMessage> messages);
        Task<bool> AddAsync(List<LocalChatMessage> messages, string conversationId);
    }

    public class ConversationRepository : IConversationRepository
    {
        private readonly IDatabase _db;

        public ConversationRepository(IDatabase db)
        {
            _db = db;
        }

        public async Task<LocalChatMessage> GetAsync(string conversationId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddAsync(List<LocalChatMessage> messages)
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

        public async Task<bool> AddAsync(List<LocalChatMessage> messages, string conversationId)
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
