using System;
using Embedly.OEmbed;

namespace Embedly
{
	public class MongoCacheItem
	{
		public Guid Id { get; private set; }
		public Uri Url { get; private set; }
		public DateTime CachedOn { get; private set; }
		public Response Response { get; private set; }

		public MongoCacheItem(Guid id, Uri url, Response response)
		{
			Id = id;
			Url = url;
			CachedOn = DateTime.UtcNow;
			Response = response;
		}
	}
}