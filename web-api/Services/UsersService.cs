// Services/UsersService.cs
using MongoDB.Driver;
using System.Collections.Generic;
using web_api.Model;
using web_api.Interfaces;
using web_api.Data;

namespace web_api.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _users;

        public UsersService(IMongoDBService mongoDBService)
        {
            _users = mongoDBService.Database.GetCollection<User>("users");
        }

        public List<User> Get() => _users.Find(user => true).ToList();

        public User Get(string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public IUser Create(User user)  // Alterado para User
        {
            // Validações de formulário
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new ArgumentException("O nome de usuário é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("O email é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("A senha é obrigatória.");
            }

            // Validação de e-mail
            var existingUser = _users.Find(u => u.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                throw new ArgumentException("O e-mail já está em uso.");
            }

            user.Id = null;  // Garante que o ID seja gerado automaticamente

            // Inserção no banco de dados
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn)
        {
            // Verifica se o ID fornecido é válido
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID inválido.");
            }

            // Verifica se o usuário a ser atualizado existe no banco de dados
            var existingUser = _users.Find(u => u.Id == id).FirstOrDefault();
            if (existingUser == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            // Validações de formulário
            if (string.IsNullOrWhiteSpace(userIn.Name))
            {
                throw new ArgumentException("O nome de usuário é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(userIn.Email))
            {
                throw new ArgumentException("O email é obrigatório.");
            }

            // Validação de e-mail
            var userWithSameEmail = _users.Find(u => u.Email == userIn.Email && u.Id != id).FirstOrDefault();
            if (userWithSameEmail != null)
            {
                throw new ArgumentException("O e-mail já está em uso.");
            }

            // Atualiza os campos do usuário, exceto o ID
            userIn.Id = existingUser.Id; // Garante que o ID não será alterado
            _users.ReplaceOne(u => u.Id == id, userIn);
        }

        public void Remove(User userIn) => _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) => _users.DeleteOne(user => user.Id == id);
    }
}
