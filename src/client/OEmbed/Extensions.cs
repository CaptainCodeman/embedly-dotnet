using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		/// <param name="timeout">The timeout.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		public static IEnumerable<Result> GetOEmbeds(this Client client, IEnumerable<Uri> urls, Func<Provider, bool> providerFilter, RequestOptions options)
		{
			if (urls == null)
				throw new ArgumentNullException("urls");

			if (options == null)
				throw new ArgumentNullException("options");

			var results = urls
				.MakeUrlRequests(client, options.SupportedOnly)
				.WhereProvider(providerFilter)
				.TakeChunks(20)
				.MakeOEmbedRequest(client.Key, options)
				.Download(client.Timeout);

			return results;
		}

		/// <summary>
		/// Create the oEmbed request.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="key">The key.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		private static IEnumerable<EmbedlyRequest> MakeOEmbedRequest(this IEnumerable<IEnumerable<UrlRequest>> source, string key, RequestOptions options)
		{
			return source.Select(reqs =>
			    new EmbedlyRequest(
					new Uri(@"http://api.embed.ly/1/oembed?format=json&key=" + key + @"&urls=" + string.Join(",", reqs.Select(req => Uri.EscapeDataString(req.Url.AbsoluteUri))) + options.GetQueryString()),
					reqs.ToArray()
				)
			);
		}

		/// <summary>
		/// Send the oEmbed request to embedly and process the result
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns></returns>
		private static IEnumerable<Result> Download(this IEnumerable<EmbedlyRequest> source, TimeSpan timeout)
		{
			if (!source.Any())
				return new Result[] {};

			var results = new BlockingCollection<Result>();
			var count = 0;

			foreach (var request in source)
			{
				Interlocked.Increment(ref count);
				HttpSocket.GetAsync(request.EmbedlyUrl.AbsoluteUri, timeout, callbackState =>
				{
					var state = (EmbedlyRequest)callbackState.State;
					if (callbackState.Exception == null)
					{
						if (callbackState.Exception == null)
						{
							var responses = Deserialize(callbackState.ResponseStream);
							for (var i = 0; i < state.UrlRequests.Length; i++)
							{
								results.Add(new Result(state.UrlRequests[i], responses[i]));
							}
						}
					}
					else
					{
						for (var i = 0; i < state.UrlRequests.Length; i++)
						{
							results.Add(new Result(state.UrlRequests[i], callbackState.Exception));
						}
					}

					// signal when we're done so consumers can finish
				    if (Interlocked.Decrement(ref count) == 0)
				        results.CompleteAdding();

				}, request);
			}

			return results.GetConsumingEnumerable();
		}

		/// <summary>
		/// Deserializes the specified stream into an array of OEmbed responses.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		private static Response[] Deserialize(Stream stream)
		{
			var serializer = new DataContractJsonSerializer(typeof(Response[]), new Type[] { }, int.MaxValue, false, new ResponseDataContractSurrogate(), false);
			var result = (Response[])serializer.ReadObject(stream);

			return result;
		}
	}
}