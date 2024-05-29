// Model/Items.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace web_api.Model
{
    public class Items
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("completed")]
        public bool Completed { get; set; } = false;
    }
}
