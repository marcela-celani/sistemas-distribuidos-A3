// Model/User.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using web_api.Interfaces;

namespace web_api.Model
{
    public class User : IUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }


        [BsonElement("name")]
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(30, ErrorMessage = "O campo Nome não pode ter mais de 30 caracteres.")]
        public string Name { get; set; }

        [BsonElement("email")]
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve conter um endereço de e-mail válido.")]
        public string Email { get; set; }

        [BsonElement("password")]
        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "O campo Senha deve ter entre 6 e 16 caracteres.")]
        public string Password { get; set; }
    }
}
