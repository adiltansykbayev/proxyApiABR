using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace proxyApiABR
{
    public class GoogleModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        public string? Link { get; set; }

        public string? SearchQuery { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }
    }
}
