using MongoDB.Driver;
using web_api.Model;
using web_api.Interfaces;
using web_api.Data;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace web_api.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Items> _items;

        public UsersService(IMongoDBService mongoDBService)
        {
            _users = mongoDBService.Database.GetCollection<User>("users");
            _items = mongoDBService.Database.GetCollection<Items>("items");
        }

        public List<User> Get() => _users.Find(user => true).ToList();

        public User Get(string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public IUser Create(User user)
        {
            ValidateUserModel(user);

            var existingUser = _users.Find(u => u.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                throw new ValidationException("O e-mail já está em uso.");
            }

            user.Id = null;  // Garante que o ID seja gerado automaticamente

            _users.InsertOne(user); 
            return user;
        }

        public void Update(string id, User userIn)
        {
            ValidateUserModel(userIn);

            var existingUser = _users.Find(u => u.Id == id).FirstOrDefault();
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            var userWithSameEmail = _users.Find(u => u.Email == userIn.Email && u.Id != id).FirstOrDefault();
            if (userWithSameEmail != null)
            {
                throw new ValidationException("O e-mail já está em uso.");
            }

            userIn.Id = existingUser.Id; // Garante que o ID não será alterado
            _users.ReplaceOne(u => u.Id == id, userIn);
        }

        public void DeleteUserAndItems(string userId)
        {
            // Excluir itens do usuário
            var filter = Builders<Items>.Filter.Eq(item => item.UserId, userId);
            _items.DeleteMany(filter);

            // Excluir o próprio usuário
            var result = _users.DeleteOne(user => user.Id == userId);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }
        }

        public void Remove(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ValidationException("ID inválido.");
            }

            var result = _users.DeleteOne(user => user.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }
        }

        public User ValidateUser(string email, string password)
        {
            return _users.Find<User>(user => user.Email == email && user.Password == password).FirstOrDefault();
        }

        private void ValidateUserModel(User user)
        {
            var validationContext = new ValidationContext(user);
            Validator.ValidateObject(user, validationContext, validateAllProperties: true);
        }

    }
}
