﻿using System;
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