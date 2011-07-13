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
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public Response Get(Guid key)
		{
			if (_cache.ContainsKey(key))
				return _cache[key];

			return null;
		}

		/// <summary>
		/// Caches the response for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Put(Guid key, Response value)
		{
			_cache.Add(key, value);
		}
	}
}