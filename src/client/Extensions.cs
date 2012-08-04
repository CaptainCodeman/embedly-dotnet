using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

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
        /// <param name="url">The URL.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        internal static UrlRequest MakeUrlRequests(this Uri url, Client client)
        {
            return new UrlRequest(client.GetProvider(url), url);
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
        internal static IObservable<UrlRequest> WhereProvider(this IObservable<UrlRequest> source, Func<Provider, bool> predicate)
		{
			return predicate == null ? source : source.Where(pr => predicate(pr.Provider));
		}
	}
}