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
        public string CreateBy { get; set; }
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
