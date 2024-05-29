// Services/ItemsService.cs
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using web_api.Model;
using web_api.Data;

namespace web_api.Services
{
    public class ItemsService
    {
        private readonly IMongoCollection<Items> _items;

        public ItemsService(MongoDBService mongoDBService)
        {
            _items = mongoDBService.Database.GetCollection<Items>("items");
        }

        public List<Items> Get() => _items.Find(item => true).ToList();

        public List<Items> GetByUserId(string userId)
        {
            return _items.Find<Items>(item => item.UserId == userId).ToList();
        }

        public Items Get(string id)
        {
            return _items.Find<Items>(item => item.Id == id).FirstOrDefault();
        }

        public Items Create(Items item)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                throw new ArgumentException("O título é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(item.Description))
            {
                throw new ArgumentException("A descrição é obrigatória.");
            }

            // Inserção no banco de dados
            _items.InsertOne(item);
            return item;
        }

        public void Update(string id, Items itemIn)
        {
            _items.ReplaceOne(item => item.Id == id, itemIn);
        }

        public void Remove(Items itemIn)
        {
            _items.DeleteOne(item => item.Id == itemIn.Id);
        }

        public void Remove(string id)
        {
            _items.DeleteOne(item => item.Id == id);
        }
    }
}