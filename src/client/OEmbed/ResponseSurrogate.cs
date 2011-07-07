using System.Runtime.Serialization;

namespace Embedly.Client.OEmbed
{
	/// <summary>
	/// Surrogate class used when deserializing embedly oEmbed response
	/// </summary>
	[DataContract]
	[KnownType(typeof(Error))]
	[KnownType(typeof(Link))]
	public class ResponseSurrogate : Response, IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }
	}
}