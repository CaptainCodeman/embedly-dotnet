using System;
using System.Collections.Generic;
using System.Linq;

namespace Embedly
{
	/// <summary>
	/// Internal LINQ extensions
	/// </summary>
	internal static class Extensions
	{
		internal static IEnumerable<UrlRequest> MakeUrlRequests(this IEnumerable<Uri> source, Client client, bool supportedOnly)
		{
			var result = source.Select(url => new UrlRequest(client.GetProvider(url), url));
			if (supportedOnly)
				result = result.Where(pr => pr.Provider != null);

			return result;
		}

		internal static IEnumerable<UrlRequest> WhereProvider(this IEnumerable<UrlRequest> source, Func<Provider, bool> predicate)
		{
			return predicate == null ? source : source.Where(pr => predicate(pr.Provider));
		}

		internal static IEnumerable<IEnumerable<T>> TakeChunks<T>(this IEnumerable<T> source, int size)
		{
			var list = new List<T>(size);

			foreach (var item in source)
			{
				list.Add(item);
				if (list.Count == size)
				{
					var chunk = list;
					list = new List<T>(size);
					yield return chunk;
				}
			}

			if (list.Count > 0)
			{
				yield return list;
			}
		}
	}
}