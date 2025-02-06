using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dot.DataAccess.Entities
{
    public class Setting<T>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Type { get; set; }

        public T Value { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
