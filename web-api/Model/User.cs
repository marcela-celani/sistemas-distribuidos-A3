// Model/User.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using web_api.Interfaces;

namespace web_api.Model
{
    public class User : IUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }
    }
}
