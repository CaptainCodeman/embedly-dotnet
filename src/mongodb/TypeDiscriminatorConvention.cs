using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Embedly
{
	internal class TypeDiscriminatorConvention : IDiscriminatorConvention
	{
		public string ElementName { get { return null; } }

		public Type GetActualType(BsonReader bsonReader, Type nominalType)
		{
			var bookmark = bsonReader.GetBookmark();
			bsonReader.ReadStartDocument();
			var actualType = nominalType;
			if (bsonReader.FindElement("Type"))
			{
				var discriminator = BsonValue.ReadFrom(bsonReader);
				actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
			}
			bsonReader.ReturnToBookmark(bookmark);
			return actualType;
		}

		public BsonValue GetDiscriminator(Type nominalType, Type actualType)
		{
			return null;
		}
	}
}