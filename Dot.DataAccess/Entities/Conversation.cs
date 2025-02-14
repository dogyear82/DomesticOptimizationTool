using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dot.DataAccess.Entities
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Message
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Role { get; set; }
        public string Content { get; set; }
        public bool IsUsedInSummary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
