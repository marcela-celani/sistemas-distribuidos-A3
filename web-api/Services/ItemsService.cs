using MongoDB.Driver;
using web_api.Model;
using web_api.Data;

namespace web_api.Services
{
    public class ItemsService
    {
        private readonly IMongoCollection<Items> _items;

        public ItemsService(IMongoDBService mongoDBService)
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

            item.Id = null;  // Garante que o ID seja gerado automaticamente

            // Inserção no banco de dados
            _items.InsertOne(item);
            return item;
        }

        public void Update(string id, Items itemIn)
        {
            // Verifica se o ID fornecido é válido
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID inválido.");
            }

            // Verifica se o item a ser atualizado existe no banco de dados
            var existingItem = _items.Find(item => item.Id == id).FirstOrDefault();
            if (existingItem == null)
            {
                throw new ArgumentException("Item não encontrado.");
            }

            // Validações
            if (string.IsNullOrWhiteSpace(itemIn.Title))
            {
                throw new ArgumentException("O título é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(itemIn.Description))
            {
                throw new ArgumentException("A descrição é obrigatória.");
            }

            // Atualiza os campos do item
            itemIn.Id = existingItem.Id; // Garante que o ID não será alterado
            _items.ReplaceOne(item => item.Id == id, itemIn);
        }

        public void Remove(string id, string userId)
        {
            var existingItem = _items.Find(item => item.Id == id).FirstOrDefault();

            if (existingItem == null)
            {
                throw new ArgumentException("Item não encontrado.");
            }

            if (existingItem.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para excluir este item.");
            }

            _items.DeleteOne(item => item.Id == id);
        }

    }
}
