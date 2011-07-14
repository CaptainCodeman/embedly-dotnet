using System;
using System.Collections.Generic;
using Embedly.OEmbed;

namespace Embedly.Caching
{
	/// <summary>
	/// Simple, dictionary based, in-memory cache implementation
	/// </summary>
	public class InMemoryResponseCache : IResponseCache
	{
		private readonly IDictionary<Guid, Response> _cache = new Dictionary<Guid, Response>();

		/// <summary>
		/// Gets the cached response for the specified key.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		public Response Get(UrlRequest request)
		{
			if (_cache.ContainsKey(request.CacheKey))
				return _cache[request.CacheKey];

			return null;
		}

		/// <summary>
		/// Caches the response for the specified key.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="value">The value.</param>
		public void Put(UrlRequest request, Response value)
		{
			_cache.Add(request.CacheKey, value);
		}
	}
}