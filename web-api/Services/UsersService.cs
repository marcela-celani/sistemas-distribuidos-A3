// Services/UsersService.cs
using MongoDB.Driver;
using System.Collections.Generic;
using web_api.Model;
using web_api.Data;

namespace web_api.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _users;

        public UsersService(MongoDBService mongoDBService)
        {
            _users = mongoDBService.Database.GetCollection<User>("users");
        }

        public List<User> Get() => _users.Find(user => true).ToList();

        public User Get(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public User Create(User user)
        {
            _users.InsertOne(user);  // O MongoDB vai gerar automaticamente o ID
            return user;
        }

        public void Update(string id, User userIn)
        {
            userIn.Id = id; // Atualiza o ID do usuário de entrada com o ID fornecido

            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn) => _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id)
        {
            _users.DeleteOne(user => user.Id == id);
        }
    }
}
