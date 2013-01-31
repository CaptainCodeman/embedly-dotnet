using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Embedly.OEmbed;

namespace Embedly.Caching
{
    public class AdoResponseCache : IResponseCache
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public AdoResponseCache(DbProviderFactory factory, string connectionString)
        {
            _factory = factory;
            _connectionString = connectionString;

            EnsureTableExists();
        }

        private void EnsureTableExists()
        {
            var sql = @"IF EXISTS(SELECT * FROM sysobjects WHERE name='EmbedlyCache' AND xtype = 'U') RETURN;
CREATE TABLE [dbo].[EmbedlyCache]
(
    [Key] [uniqueidentifier] NOT NULL,
    [Url] [varchar](2000) NOT NULL,
    [CachedOn] [DateTime] NOT NULL,
    [Value] [varchar](max) NOT NULL,

    CONSTRAINT [PK_EmbedlyCache] PRIMARY KEY CLUSTERED ([Key] ASC)
);";

            using (DbConnection connection = _factory.CreateConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        #region IResponseCache Members

        public Response Get(UrlRequest request)
        {
            using (DbConnection connection = _factory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "select [Value] from EmbedlyCache where [Key] = @key";

                DbParameter pKey = cmd.CreateParameter();
                pKey.DbType = DbType.Guid;
                pKey.Direction = ParameterDirection.Input;
                pKey.ParameterName = "key";
                pKey.Value = request.CacheKey;

                cmd.Parameters.Add(pKey);

                var value = (string)cmd.ExecuteScalar();
                if (value == null)
                    return null;

                byte[] bytes = Encoding.Default.GetBytes(value);
                using (var ms = new MemoryStream(bytes))
                {
                    var serializer = new DataContractJsonSerializer(typeof (Response));
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
                var serializer = new DataContractJsonSerializer(typeof (Response));
                serializer.WriteObject(ms, value);
                ms.Position = 0;
                valueString = Encoding.Default.GetString(ms.ToArray());
            }

            using (DbConnection connection = _factory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "insert into EmbedlyCache ([Key], [Url], [CachedOn], [Value]) values (@key, @url, @cachedOn, @value)";

                DbParameter pKey = cmd.CreateParameter();
                pKey.DbType = DbType.Guid;
                pKey.Direction = ParameterDirection.Input;
                pKey.ParameterName = "key";
                pKey.Value = request.CacheKey;

                DbParameter pUrl = cmd.CreateParameter();
                pUrl.DbType = DbType.AnsiString;
                pUrl.Direction = ParameterDirection.Input;
                pUrl.ParameterName = "url";
                pUrl.Value = request.Url.AbsoluteUri;

                DbParameter pCachedOn = cmd.CreateParameter();
                pCachedOn.DbType = DbType.DateTime;
                pCachedOn.Direction = ParameterDirection.Input;
                pCachedOn.ParameterName = "cachedOn";
                pCachedOn.Value = DateTime.UtcNow;

                DbParameter pValue = cmd.CreateParameter();
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

        #endregion
    }
}