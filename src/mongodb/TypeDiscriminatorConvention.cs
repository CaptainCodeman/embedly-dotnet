using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Embedly
{
    internal class TypeDiscriminatorConvention : IDiscriminatorConvention
    {
        #region IDiscriminatorConvention Members

        public string ElementName
        {
            get { return null; }
        }

        public Type GetActualType(BsonReader bsonReader, Type nominalType)
        {
            BsonReaderBookmark bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            Type actualType = nominalType;
            if (bsonReader.FindElement("Type"))
            {
                BsonValue discriminator = BsonValue.ReadFrom(bsonReader);
                actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
            }
            bsonReader.ReturnToBookmark(bookmark);
            return actualType;
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            return null;
        }

        #endregion
    }
}