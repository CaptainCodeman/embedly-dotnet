using System;
using Embedly.OEmbed;

namespace Embedly
{
	public class MongoCacheItem
	{
		public Guid Id { get; private set; }
		public Response Response { get; private set; }

		public MongoCacheItem(Guid id, Response response)
		{
			Id = id;
			Response = response;
		}
	}
}