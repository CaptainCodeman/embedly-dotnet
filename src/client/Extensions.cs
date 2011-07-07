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
		internal static IEnumerable<UrlRequest> SupportedUrls(this IEnumerable<Uri> source)
		{
			return source.Select(url => new UrlRequest(Service.Instance.GetProvider(url), url)).Where(pr => pr.Provider != null);
		}

		internal static IEnumerable<UrlRequest> WhereProvider(this IEnumerable<UrlRequest> source, Func<Provider, bool> predicate)
		{
			return predicate == null ? source : source.Where(pr => predicate(pr.Provider));
		}

		internal static IEnumerable<IEnumerable<T>> TakeChunks<T>(this IEnumerable<T> source, int size)
		{
			var list = new List<T>(size);

			foreach (T item in source)
			{
				list.Add(item);
				if (list.Count == size)
				{
					List<T> chunk = list;
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