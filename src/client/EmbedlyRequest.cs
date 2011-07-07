using System;

namespace Embedly
{
	/// <summary>
	/// Represents a single call to embedly
	/// </summary>
	internal class EmbedlyRequest
	{
		/// <summary>
		/// Gets the embedly URL (used for the actual HTTP request).
		/// </summary>
		public Uri EmbedlyUrl { get; private set; }

		/// <summary>
		/// Gets the URL requests (these are the URLs we're looking up).
		/// </summary>
		public UrlRequest[] UrlRequests { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbedlyRequest"/> class.
		/// </summary>
		/// <param name="embedlyUrl">The embedly URL.</param>
		/// <param name="urlRequests">The URL requests.</param>
		public EmbedlyRequest(Uri embedlyUrl, UrlRequest[] urlRequests)
		{
			EmbedlyUrl = embedlyUrl;
			UrlRequests = urlRequests;
		}
	}
}