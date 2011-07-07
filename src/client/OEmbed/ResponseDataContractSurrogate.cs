using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Embedly.OEmbed
{
	/// <summary>
	/// Used to identify and convert types when deserializing response
	/// </summary>
	public class ResponseDataContractSurrogate : IDataContractSurrogate
	{
		/// <summary>
		/// During serialization, deserialization, and schema import and export, returns a data contract type that substitutes the specified type.
		/// </summary>
		/// <param name="type">The CLR type <see cref="T:System.Type"/> to substitute.</param>
		/// <returns>
		/// The <see cref="T:System.Type"/> to substitute for the <paramref name="type"/> value. This type must be serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/>. For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes.
		/// </returns>
		public Type GetDataContractType(Type type)
		{
			if (typeof (Response).IsAssignableFrom(type))
			{
				return typeof (ResponseSurrogate);
			}

			return type;
		}

		/// <summary>
		/// During serialization, returns an object that substitutes the specified object.
		/// </summary>
		/// <param name="obj">The object to substitute.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> that the substituted object should be assigned to.</param>
		/// <returns>
		/// The substituted object that will be serialized. The object must be serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/>. For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes.
		/// </returns>
		public object GetObjectToSerialize(object obj, Type targetType)
		{
			return obj;
		}

		/// <summary>
		/// During deserialization, returns an object that is a substitute for the specified object.
		/// </summary>
		/// <param name="obj">The deserialized object to be substituted.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> that the substituted object should be assigned to.</param>
		/// <returns>
		/// The substituted deserialized object. This object must be of a type that is serializable by the <see cref="T:System.Runtime.Serialization.DataContractSerializer"/>. For example, it must be marked with the <see cref="T:System.Runtime.Serialization.DataContractAttribute"/> attribute or other mechanisms that the serializer recognizes.
		/// </returns>
		public object GetDeserializedObject(object obj, Type targetType)
		{
			var response = obj as ResponseSurrogate;
			if (response != null)
			{
				Type typeClass;

				switch (response.Type)
				{
					case ResourceType.Error:
						typeClass = typeof (Error);
						break;
					case ResourceType.Link:
						typeClass = typeof (Link);
						break;
					case ResourceType.Photo:
						typeClass = typeof (Photo);
						break;
					case ResourceType.Rich:
						typeClass = typeof (Rich);
						break;
					case ResourceType.Video:
						typeClass = typeof (Video);
						break;

					default:
						throw new SerializationException(string.Format("Unrecognized type {0}", response.Type));
				}

				using (var ms = new MemoryStream())
				{
					var serializer = new DataContractJsonSerializer(typeof(ResponseSurrogate));
					serializer.WriteObject(ms, response);
					ms.Position = 0;

					serializer = new DataContractJsonSerializer(typeClass);
					obj = serializer.ReadObject(ms);
				}
			}

			return obj;
		}

		/// <summary>
		/// During schema import, returns the type referenced by the schema.
		/// </summary>
		/// <param name="typeName">The name of the type in schema.</param>
		/// <param name="typeNamespace">The namespace of the type in schema.</param>
		/// <param name="customData">The object that represents the annotation inserted into the XML schema definition, which is data that can be used for finding the referenced type.</param>
		/// <returns>
		/// The <see cref="T:System.Type"/> to use for the referenced type.
		/// </returns>
		public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
		{
			return null;
		}

		/// <summary>
		/// Processes the type that has been generated from the imported schema.
		/// </summary>
		/// <param name="typeDeclaration">A <see cref="T:System.CodeDom.CodeTypeDeclaration"/> to process that represents the type declaration generated during schema import.</param>
		/// <param name="compileUnit">The <see cref="T:System.CodeDom.CodeCompileUnit"/> that contains the other code generated during schema import.</param>
		/// <returns>
		/// A <see cref="T:System.CodeDom.CodeTypeDeclaration"/> that contains the processed type.
		/// </returns>
		public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
		{
			return typeDeclaration;
		}

		/// <summary>
		/// During schema export operations, inserts annotations into the schema for non-null return values.
		/// </summary>
		/// <param name="clrType">The CLR type to be replaced.</param>
		/// <param name="dataContractType">The data contract type to be annotated.</param>
		/// <returns>
		/// An object that represents the annotation to be inserted into the XML schema definition.
		/// </returns>
		public object GetCustomDataToExport(Type clrType, Type dataContractType)
		{
			return null;
		}

		/// <summary>
		/// During schema export operations, inserts annotations into the schema for non-null return values.
		/// </summary>
		/// <param name="memberInfo">A <see cref="T:System.Reflection.MemberInfo"/> that describes the member.</param>
		/// <param name="dataContractType">A <see cref="T:System.Type"/>.</param>
		/// <returns>
		/// An object that represents the annotation to be inserted into the XML schema definition.
		/// </returns>
		public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
		{
			return null;
		}

		/// <summary>
		/// Sets the collection of known types to use for serialization and deserialization of the custom data objects.
		/// </summary>
		/// <param name="customDataTypes">A <see cref="T:System.Collections.ObjectModel.Collection`1"/>  of <see cref="T:System.Type"/> to add known types to.</param>
		public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
		{
			return;
		}
	}
}