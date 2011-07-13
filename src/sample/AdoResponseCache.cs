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

		public Response Get(Guid key)
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
				pKey.Value = key;

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

		public void Put(Guid key, Response value)
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
				cmd.CommandText = "insert into Response ([key], [value]) values (@key, @value)";

				var pKey = cmd.CreateParameter();
				pKey.DbType = DbType.Guid;
				pKey.Direction = ParameterDirection.Input;
				pKey.ParameterName = "key";
				pKey.Value = key;

				var pValue = cmd.CreateParameter();
				pValue.DbType = DbType.String;
				pValue.Direction = ParameterDirection.Input;
				pValue.ParameterName = "value";
				pValue.Value = valueString;

				cmd.Parameters.Add(pKey);
				cmd.Parameters.Add(pValue);

				cmd.ExecuteNonQuery();
			}
		}
	}
}
