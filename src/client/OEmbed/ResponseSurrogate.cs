using System.Runtime.Serialization;

namespace Embedly.OEmbed
{
	/// <summary>
	/// Surrogate class used when deserializing embedly oEmbed response
	/// </summary>
	[DataContract]
	public class ResponseSurrogate : IExtensibleDataObject
	{
		[DataMember(Name = "type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets the version.
		/// </summary>
		[DataMember(Name = "version")]
		public string Version { get; private set; }

		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }
	}
}