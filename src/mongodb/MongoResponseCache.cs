using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Embedly.Caching;
using Embedly.OEmbed;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Embedly
{
	public class MongoResponseCache : IResponseCache
	{
		private readonly MongoCollection<MongoCacheItem> _collection;

		public MongoResponseCache(string connectionString)
		{
			BsonSerializer.RegisterDiscriminatorConvention(typeof(Response), new TypeDiscriminatorConvention());

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

			var database = MongoDatabase.Create(connectionString);
			var settings = new MongoCollectionSettings<MongoCacheItem>(database, "embedly");
			_collection = database.GetCollection(settings);
		}

		public Response Get(UrlRequest request)
		{
			var cacheItem = _collection.FindOneById(request.CacheKey);
			return cacheItem == null ? null : cacheItem.Response;
		}

		public void Put(UrlRequest request, Response response)
		{
			var cacheItem = new MongoCacheItem(request.CacheKey, request.Url, response);
			_collection.Insert(cacheItem, SafeMode.False);
		}
	}
}
