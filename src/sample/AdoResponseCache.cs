using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Embedly.Caching;
using Embedly.OEmbed;

namespace Embedly.Sample
{
	public class AdoResponseCache : IResponseCache
	{
		private readonly DbProviderFactory _factory;
		private readonly string _connectionString;
		
		public AdoResponseCache(DbProviderFactory factory, string connectionString)
		{
			_factory = factory;
			_connectionString = connectionString;
		}

		public Response Get(UrlRequest request)
		{
			using (var connection = _factory.CreateConnection())
			{
				connection.ConnectionString = _connectionString;
				connection.Open();

				var cmd = connection.CreateCommand();
				cmd.CommandText = "select [Value] from Response where [Key] = @key";
				
				var pKey = cmd.CreateParameter();
				pKey.DbType = DbType.Guid;
				pKey.Direction = ParameterDirection.Input;
				pKey.ParameterName = "key";
				pKey.Value = request.CacheKey;

				cmd.Parameters.Add(pKey);

				var value = (string)cmd.ExecuteScalar();
				if (value == null)
					return null;

				var bytes = Encoding.Default.GetBytes(value);
				using (var ms = new MemoryStream(bytes))
				{
					var serializer = new DataContractJsonSerializer(typeof(Response));
					var response = (Response)serializer.ReadObject(ms);
					return response;
				}
			}
		}

		public void Put(UrlRequest request, Response value)
		{
			string valueString;
			using (var ms = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(typeof(Response));
				serializer.WriteObject(ms, value);
				ms.Position = 0;
				valueString = Encoding.Default.GetString(ms.ToArray());
			}

			using (var connection = _factory.CreateConnection())
			{
				connection.ConnectionString = _connectionString;
				connection.Open();

				var cmd = connection.CreateCommand();
				cmd.CommandText = "insert into Response ([Key], [Url], [CachedOn], [Value]) values (@key, @url, @cachedOn, @value)";

				var pKey = cmd.CreateParameter();
				pKey.DbType = DbType.Guid;
				pKey.Direction = ParameterDirection.Input;
				pKey.ParameterName = "key";
				pKey.Value = request.CacheKey;

				var pUrl = cmd.CreateParameter();
				pUrl.DbType = DbType.AnsiString;
				pUrl.Direction = ParameterDirection.Input;
				pUrl.ParameterName = "url";
				pUrl.Value = request.Url.AbsoluteUri;

				var pCachedOn = cmd.CreateParameter();
				pCachedOn.DbType = DbType.DateTime;
				pCachedOn.Direction = ParameterDirection.Input;
				pCachedOn.ParameterName = "cachedOn";
				pCachedOn.Value = DateTime.UtcNow;

				var pValue = cmd.CreateParameter();
				pValue.DbType = DbType.String;
				pValue.Direction = ParameterDirection.Input;
				pValue.ParameterName = "value";
				pValue.Value = valueString;

				cmd.Parameters.Add(pKey);
				cmd.Parameters.Add(pUrl);
				cmd.Parameters.Add(pCachedOn);
				cmd.Parameters.Add(pValue);

				cmd.ExecuteNonQuery();
			}
		}
	}
}
