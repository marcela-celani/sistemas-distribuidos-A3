using MongoDB.Driver;

namespace web_api.Data
{
    public class MongoDBService
    {
        private readonly IConfiguration _config;
        private readonly IMongoDatabase? _database;
        public MongoDBService(IConfiguration configuration) {
            _config = configuration;

            var connectionString = _config.GetConnectionString("DbConnection");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);

            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase? Database => _database;
    }
}
