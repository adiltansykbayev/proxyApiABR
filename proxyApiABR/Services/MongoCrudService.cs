using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace proxyApiABR.Services
{
    public class MongoCrudService
    {
        private readonly IMongoCollection<GoogleModel> _collection;

        public MongoCrudService(
            IOptions<DatabaseSettings> mongoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                mongoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                mongoDatabaseSettings.Value.DatabaseName);

            _collection = mongoDatabase.GetCollection<GoogleModel>(
                mongoDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<GoogleModel>> GetAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<GoogleModel?> GetAsync(string id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<GoogleModel?> GetByQueryAsync(string query) =>
            await _collection.Find(x => x.SearchQuery == query).FirstOrDefaultAsync();
        public async Task CreateAsync(GoogleModel newItem) =>
            await _collection.InsertOneAsync(newItem);

        public async Task UpdateAsync(string id, GoogleModel item) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, item);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
