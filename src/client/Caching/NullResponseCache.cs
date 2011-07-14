using System;
using Embedly.OEmbed;

namespace Embedly.Caching
{
	/// <summary>
	/// Null response cache implementation for no caching
	/// </summary>
	public class NullResponseCache : IResponseCache
	{
		public Response Get(UrlRequest request)
		{
			return null;
		}

		public void Put(UrlRequest request, Response response)
		{
			// no-op
		}
	}
}