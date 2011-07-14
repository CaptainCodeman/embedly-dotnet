using System;
using Embedly.OEmbed;

namespace Embedly.Caching
{
	/// <summary>
	/// Interface for cache providers
	/// </summary>
	public interface IResponseCache
	{
		/// <summary>
		/// Gets the cached response for the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		Response Get(UrlRequest request);

		/// <summary>
		/// Caches the response for the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="response">The response.</param>
		void Put(UrlRequest request, Response response);
	}
}
