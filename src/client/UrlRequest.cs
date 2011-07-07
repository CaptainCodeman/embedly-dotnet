using System;

namespace Embedly.Client
{
	/// <summary>
	/// Represents an individual embedly url request with the associated provider that is expected to fulfill it
	/// </summary>
	internal class UrlRequest
	{
		/// <summary>
		/// Gets or sets the provider.
		/// </summary>
		/// <value>
		/// The provider.
		/// </value>
		internal Provider Provider { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		internal Uri Url { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlRequest"/> class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <param name="url">The URL.</param>
		internal UrlRequest(Provider provider, Uri url)
		{
			Provider = provider;
			Url = url;
		}
	}
}