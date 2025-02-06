using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dot.DataAccess.Entities
{
    public class Settings<T>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // "api-settings", "ui-settings"

        public string Type { get; set; } // "api", "ui", "worker"

        public T Value { get; set; } // Typed settings

        public DateTime CreatedAt { get; set; }
    }
}
