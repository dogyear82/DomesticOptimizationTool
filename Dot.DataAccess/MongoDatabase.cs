using Dot.DataAccess.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dot.DataAccess
{
    public class MongoDatabase : IDatabase
    {
        private readonly IMongoClient _client;
        private readonly MongoDbSettings _dbSettings;

        public MongoDatabase(IMongoClient client, IOptionsMonitor<MongoDbSettings> options)
        {
            _client = client;
            _dbSettings = options.CurrentValue;
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            var collectionName = $"{typeof(T).Name.ToLower().Split('`')[0]}s";
            return _client.GetDatabase(_dbSettings.DatabaseName).GetCollection<T>(collectionName);
        }

        public async Task<bool> CreateAsync<T>(T record)
        {
            var collection = GetCollection<T>();
            await collection.InsertOneAsync(record);
            return true;
        }

        public async Task<List<T>> ReadAsync<T>()
        {
            return await GetCollection<T>().Find(_ => true).ToListAsync();
        }

        public async Task<T> ReadAsync<T>(string id)
        {
            return await Find<T>("_id", new ObjectId(id)).FirstOrDefaultAsync();
        }

        public async Task<List<T>> ReadAsync<T>(string filterName, string filterValue)
        {
            return Find<T>(filterName, filterValue).ToList();
        }

        private IFindFluent<T, T> Find<T>(string filterName, object filterValue)
        {
            var filter = Builders<T>.Filter.Eq(filterName, filterValue);
            return GetCollection<T>().Find(filter);
        }

        public async Task<bool> UpdateAsync<T>(string id, T record)
        {
            var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
            var result = await GetCollection<T>().ReplaceOneAsync(filter, record);
            return result.ModifiedCount > 0;
        }
    }
}
