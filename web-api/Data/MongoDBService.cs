using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace web_api.Data
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection");
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);
            var databaseName = client.GetDatabase(configuration["DatabaseName"]);

            _database = databaseName;
        }

        public IMongoDatabase Database => _database;
    }
}
