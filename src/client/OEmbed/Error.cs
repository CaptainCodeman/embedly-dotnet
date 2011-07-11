using System.Runtime.Serialization;

namespace Embedly.OEmbed
{
	/// <summary>
	/// Represents an oEmbed error response
	/// </summary>
	[DataContract]
	public class Error : Response
	{
		/// <summary>
		/// Gets the error code.
		/// </summary>
		[DataMember(Name = "error_code")]
		public string ErrorCode { get; private set; }

		/// <summary>
		/// Gets the error message.
		/// </summary>
		[DataMember(Name = "error_message")]
		public string ErrorMessage { get; private set; }
	}
}