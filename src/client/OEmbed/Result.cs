using System;
using System.Net;

namespace Embedly.OEmbed
{
	/// <summary>
	/// Represents an oEmbed result
	/// </summary>
	public class Result
	{
		/// <summary>
		/// Gets the originally requested URL.
		/// </summary>
		public Uri RequestedUrl { get; private set; }

		/// <summary>
		/// Gets the response.
		/// </summary>
		public Response Response { get; private set; }

		/// <summary>
		/// Gets the exception.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Result"/> class.
		/// </summary>
		/// <param name="requestedUrl">The requested URL.</param>
		/// <param name="response">The response.</param>
		public Result(Uri requestedUrl, Response response)
		{
			RequestedUrl = requestedUrl;
			Response = response;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Result"/> class.
		/// </summary>
		/// <param name="requestedUrl">The requested URL.</param>
		/// <param name="exception">The exception.</param>
		public Result(Uri requestedUrl, Exception exception)
		{
			RequestedUrl = requestedUrl;
			Exception = exception;
		}
	}
}