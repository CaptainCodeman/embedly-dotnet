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
		private readonly string _connectionString;
		private readonly MongoCollectionSettings<MongoCacheItem> _settings;

		public MongoResponseCache(string connectionString)
		{
			_connectionString = connectionString;
			_settings = new MongoCollectionSettings<MongoCacheItem>("embedly", false, BsonDefaults.GuidRepresentation, SafeMode.False, true);

			BsonDefaultSerializer.RegisterDiscriminatorConvention(typeof(Response), new TypeDiscriminatorConvention());

			BsonClassMap.RegisterClassMap<Response>(cm =>
			{
				cm.AutoMap();
				cm.MapProperty(x => x.Type).SetRepresentation(BsonType.String);
				cm.SetIgnoreExtraElements(true);
			});

			BsonClassMap.RegisterClassMap<Error>();
			BsonClassMap.RegisterClassMap<Link>();
			BsonClassMap.RegisterClassMap<Photo>();
			BsonClassMap.RegisterClassMap<Rich>();
			BsonClassMap.RegisterClassMap<Video>();
		}

		public Response Get(Guid key)
		{
			var database = MongoDatabase.Create(_connectionString);
			var collection = database.GetCollection(_settings);
			var cacheItem = collection.FindOneById(key);
			return cacheItem == null ? null : cacheItem.Response;
		}

		public void Put(Guid key, Response value)
		{
			var database = MongoDatabase.Create(_connectionString);
			var collection = database.GetCollection(_settings);

			var cacheItem = new MongoCacheItem(key, value);
			collection.Insert(cacheItem);
		}
	}
}
