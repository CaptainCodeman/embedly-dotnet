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
		/// Gets the cached response for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		Response Get(Guid key);
		
		/// <summary>
		/// Caches the response for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Put(Guid key, Response value);
	}
}
