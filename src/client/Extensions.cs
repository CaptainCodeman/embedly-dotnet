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
		/// <summary>
		/// Create a UrlRequest instance from a Url.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="client">The client.</param>
		/// <returns></returns>
		internal static IEnumerable<UrlRequest> MakeUrlRequests(this IEnumerable<Uri> source, Client client)
		{
			return source.Select(url => new UrlRequest(client.GetProvider(url), url));
		}

		/// <summary>
		/// Extension to enable Urls to be filteres based on the provider.
		/// </summary>
		/// <remarks>
		/// The reason this is used instead of a .Where() on the final result is that we want to filter
		/// the urls before they are sent to embedly in a request
		/// </remarks>
		/// <param name="source">The source.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		internal static IEnumerable<UrlRequest> WhereProvider(this IEnumerable<UrlRequest> source, Func<Provider, bool> predicate)
		{
			return predicate == null ? source : source.Where(pr => predicate(pr.Provider));
		}

		/// <summary>
		/// Chunks the source into batches.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
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