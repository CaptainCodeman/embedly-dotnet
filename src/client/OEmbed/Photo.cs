using System.Runtime.Serialization;

namespace Embedly.Client.OEmbed
{
	/// <summary>
	/// Represents an oEmbed photo response
	/// </summary>
	[DataContract]
	public class Photo : Link
	{
		/// <summary>
		/// Gets the width.
		/// </summary>
		[DataMember(Name = "width")]
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height.
		/// </summary>
		[DataMember(Name = "height")]
		public int Height { get; private set; }
	}
}