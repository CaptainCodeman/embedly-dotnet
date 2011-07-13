using System;
using Embedly.OEmbed;

namespace Embedly.Caching
{
	/// <summary>
	/// Null response cache implementation for no caching
	/// </summary>
	public class NullResponseCache : IResponseCache
	{
		public Response Get(Guid key)
		{
			return null;
		}

		public void Put(Guid key, Response value)
		{
			// no-op
		}
	}
}