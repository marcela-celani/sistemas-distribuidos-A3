using MongoDB.Driver;

namespace web_api.Data
{
    public interface IMongoDBService
    {
        IMongoDatabase Database { get; }
    }
}
