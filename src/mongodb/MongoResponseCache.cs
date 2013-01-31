using Embedly.OEmbed;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Embedly.Caching
{
    public class MongoResponseCache : IResponseCache
    {
        private readonly MongoCollection<MongoCacheItem> _collection;

        public MongoResponseCache(string connectionString)
        {
            BsonSerializer.RegisterDiscriminatorConvention(typeof (Response), new TypeDiscriminatorConvention());

            BsonClassMap.RegisterClassMap<Response>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(x => x.Type).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<Error>();
            BsonClassMap.RegisterClassMap<Link>();
            BsonClassMap.RegisterClassMap<Photo>();
            BsonClassMap.RegisterClassMap<Rich>();
            BsonClassMap.RegisterClassMap<Video>();

            var url = MongoUrl.Create(connectionString);
            var client = new MongoClient(url);
            var server = client.GetServer();
            var database = server.GetDatabase(url.DatabaseName);

            var settings = new MongoCollectionSettings<MongoCacheItem>(database, "embedly");
            _collection = database.GetCollection(settings);
        }

        #region IResponseCache Members

        public Response Get(UrlRequest request)
        {
            MongoCacheItem cacheItem = _collection.FindOneById(request.CacheKey);
            return cacheItem == null ? null : cacheItem.Response;
        }

        public void Put(UrlRequest request, Response response)
        {
            var cacheItem = new MongoCacheItem(request.CacheKey, request.Url, response);
            _collection.Insert(cacheItem);
        }

        #endregion
    }
}