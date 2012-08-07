using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using Embedly.Http;

namespace Embedly.OEmbed
{
	/// <summary>
	/// oEmbed specific extension methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Gets an individual oEmbed.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public static Result GetOEmbed(this Client client, Uri url)
		{
			return GetOEmbed(client, url, null, new RequestOptions());
		}

		/// <summary>
		/// Gets an individual oEmbed.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="url">The URL.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public static Result GetOEmbed(this Client client, Uri url, RequestOptions options)
		{
			return GetOEmbed(client, url, null, options);
		}

		/// <summary>
		/// Gets an individual oEmbed.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="url">The URL.</param>
		/// <param name="providerFilter">The provider filter.</param>
		/// <returns></returns>
		public static Result GetOEmbed(this Client client, Uri url, Func<Provider, bool> providerFilter)
		{
			return GetOEmbed(client, url, providerFilter, new RequestOptions());
		}

		/// <summary>
		/// Gets an individual oEmbed.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="url">The URL.</param>
		/// <param name="providerFilter">The provider filter.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public static Result GetOEmbed(this Client client, Uri url, Func<Provider, bool> providerFilter, RequestOptions options)
		{
			return GetOEmbeds(client, new[] { url }, providerFilter, options).FirstOrDefault();
		}

		/// <summary>
		/// Gets multiple oEmbeds
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="urls">The urls.</param>
		/// <returns></returns>
		public static IEnumerable<Result> GetOEmbeds(this Client client, IEnumerable<Uri> urls)
		{
			return GetOEmbeds(client, urls, null, new RequestOptions());
		}

		/// <summary>
		/// Gets multiple oEmbeds
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="urls">The urls.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public static IEnumerable<Result> GetOEmbeds(this Client client, IEnumerable<Uri> urls, RequestOptions options)
		{
			return GetOEmbeds(client, urls, null, options);
		}

		/// <summary>
		/// Gets multiple oEmbeds
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="urls">The urls.</param>
		/// <param name="providerFilter">The provider filter.</param>
		/// <returns></returns>
		public static IEnumerable<Result> GetOEmbeds(this Client client, IEnumerable<Uri> urls, Func<Provider, bool> providerFilter)
		{
			return GetOEmbeds(client, urls, providerFilter, new RequestOptions());
		}

        /// <summary>
        /// Gets multiple oEmbeds
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="urls">The urls.</param>
        /// <param name="providerFilter">The provider filter.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IEnumerable<Result> GetOEmbeds(this Client client, IEnumerable<Uri> urls, Func<Provider, bool> providerFilter, RequestOptions options)
        {
            if (urls == null)
                throw new ArgumentNullException("urls");

            if (options == null)
                throw new ArgumentNullException("options");

            var results = new BlockingCollection<Result>();
            var redirector = new RequestObserver(client, options);

            redirector.Output.Subscribe(results.Add, results.CompleteAdding);

            var requests = urls
                .ToObservable(NewThreadScheduler.Default)
                .Select(u => u.MakeUrlRequests(client))
                .WhereProvider(providerFilter);

            requests.Subscribe(redirector);

            return results.GetConsumingEnumerable();
        }

        /// <summary>
        /// Gets multiple oEmbeds
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="urls">The urls.</param>
        /// <param name="providerFilter">The provider filter.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IObservable<Result> GetOEmbeds(this Client client, IObservable<Uri> urls, Func<Provider, bool> providerFilter, RequestOptions options)
        {
            if (urls == null)
                throw new ArgumentNullException("urls");

            if (options == null)
                throw new ArgumentNullException("options");

            var requests = urls
                .ObserveOn(NewThreadScheduler.Default)
                .Select(u => u.MakeUrlRequests(client))
                .WhereProvider(providerFilter);

            var redirector = new RequestObserver(client, options);

            requests.Subscribe(redirector);

            return redirector.Output;
        }

		/// <summary>
		/// Returns successful results (not exeption during request and no error from embedly).
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Successful(this IEnumerable<Result> source)
		{
			return source.Where(result => result.Exception == null && result.Response.Type != ResourceType.Error);
		}

		/// <summary>
		/// Returns failed results (exception thrown during request).
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Failed(this IEnumerable<Result> source)
		{
			return source.Where(result => result.Exception != null);
		}

		/// <summary>
		/// Returns error results (request successful but error returned by embedly for Url).
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Errors(this IEnumerable<Result> source)
		{
			return source.Successful().Where(result => result.Response.Type == ResourceType.Error);
		}

		/// <summary>
		/// Return link results.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Links(this IEnumerable<Result> source)
		{
			return source.Successful().Where(result => result.Response.Type == ResourceType.Link);
		}

		/// <summary>
		/// Return photo results.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Photos(this IEnumerable<Result> source)
		{
			return source.Successful().Where(result => result.Response.Type == ResourceType.Photo);
		}

		/// <summary>
		/// Return rich content results.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Richs(this IEnumerable<Result> source)
		{
			return source.Successful().Where(result => result.Response.Type == ResourceType.Rich);
		}

		/// <summary>
		/// Return video results.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns></returns>
		public static IEnumerable<Result> Videos(this IEnumerable<Result> source)
		{
			return source.Successful().Where(result => result.Response.Type == ResourceType.Video);
		}
	}
}