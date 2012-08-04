using System;
using System.Collections.Generic;

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
		internal Uri EmbedlyUrl { get; private set; }

		/// <summary>
		/// Gets the URL requests (these are the URLs we're looking up).
		/// </summary>
		internal IList<UrlRequest> UrlRequests { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbedlyRequest"/> class.
		/// </summary>
		/// <param name="embedlyUrl">The embedly URL.</param>
		/// <param name="urlRequests">The URL requests.</param>
        internal EmbedlyRequest(Uri embedlyUrl, IList<UrlRequest> urlRequests)
		{
			EmbedlyUrl = embedlyUrl;
			UrlRequests = urlRequests;
		}
	}
}